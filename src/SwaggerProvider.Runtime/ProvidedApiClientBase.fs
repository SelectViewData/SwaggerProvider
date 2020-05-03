namespace Swagger

open System
open System.Net.Http
open System.Text.Json
open System.Text.Json.Serialization

type OpenApiException(code:System.Net.HttpStatusCode, description:string, body: string) =
    inherit Exception(description)
    member __.StatusCode = code
    member __.Description = description
    member __.Body = body

type ProvidedApiClientBase(httpClient: HttpClient, options: JsonSerializerOptions) =

#if TP_RUNTIME
    let options =
        if isNull options then
            let options = JsonSerializerOptions()
            options.Converters.Add(JsonFSharpConverter())
            options
        else options
#endif

    member val HttpClient = httpClient with get, set

    abstract member Serialize: obj -> string
    abstract member Deserialize: string * Type -> obj

    default __.Serialize(value:obj): string =
        JsonSerializer.Serialize(value, options)
    default __.Deserialize(value, retTy:Type): obj =
        JsonSerializer.Deserialize(value, retTy, options)

    // This code may change in the future, especially when task{} become part of FSharp.Core.dll
    member this.CallAsync(request: HttpRequestMessage, errorCodes:string[], errorDescriptions:string[]) : Async<HttpContent> =
        async {
            let! response = this.HttpClient.SendAsync(request) |> Async.AwaitTask
            if response.IsSuccessStatusCode
            then return response.Content
            else
                let code = response.StatusCode
                let codeStr = code |> string
                let errorDescriptionOpt =  
                  errorCodes
                  |> Array.tryFindIndex((=)codeStr)
                  |> Option.map (fun idx -> errorDescriptions.[idx])
                let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
                
                raise (OpenApiException(code, (errorDescriptionOpt |> Option.defaultValue "Unknown Error Code"), content))
                return response.Content
        }
