module Microsoft.Bot.FunSample.SimpleSandwichBot.WebApiConfig

open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open System.Web.Http

type HttpDefaultRoute = { id : RouteParameter }

let Register (config: HttpConfiguration) =
    // Json settings
    config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling <- NullValueHandling.Ignore
    config.Formatters.JsonFormatter.SerializerSettings.ContractResolver <- CamelCasePropertyNamesContractResolver()
    config.Formatters.JsonFormatter.SerializerSettings.Formatting <- Formatting.Indented
    JsonConvert.DefaultSettings <- fun _ -> 
        JsonSerializerSettings(
            ContractResolver = CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore )

    // Web API configuration and services

    // Web API routes
    config.MapHttpAttributeRoutes()

    config.Routes.MapHttpRoute(
        name = "DefaultApi",
        routeTemplate = "api/{controller}/{id}",
        defaults = {id = RouteParameter.Optional} ) |> ignore
