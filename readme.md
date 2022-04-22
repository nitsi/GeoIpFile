# General
This is a sample project that uses .net core 5.0 MVC API with container technology.  
It has a single API `/v1/find-country?ip={IP Address}`

All code classes uses DI.

# Flow of getting an IP
GeoController (GetCountryByIpAsync) -> GeoHandler (GetFirstOrDefaultGeoInformationByIpAsync) -> GeoProvidersManagement (GetActiveProviders) -> JsonGeoProvider (GetGeoInformationByIpAsync)

# Classes
## GeoController
The controller that gets all the API requests.

## GeoHandler
Runs all providers in parallel to find a result the fastest way.

## GeoProvidersManagement
Manage a list of geo providers and is thread safe.

## JsonGeoProvider
Reads data from a JSON file to memory and cache the results.

## ThrottlingMiddleware
Intercepts requests and throttle them in case of a flood by a specific customer (source IP).

## WebApiExceptionFilter
Handles user friendly messages on exceptions.