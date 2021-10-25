# ECB-API-CacheServer

# Solution structure

Based on https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures#clean-architecture

## Core
### Database models

- **CurrencyPair** - self-explanatory

- **CacheRecord** - holds period of time between which exchange rates for given CurrencyPair are stored locally and complete

- **ExchangeRate** - holds value of exchange rate for given CurrencyPair and Date

### Repository interfaces

Used to represent allowed operations on models.

- **ICacheRecord Get(currency, startPeriod, endPeriod)** is to be used to determine if relevant exchange rates are already stored locally - if no result is returned then missing exchange rates should be fetched from ECB API.


## Infrastructure
### Database
- **CacheServerDbContext** - basic configuration to enforce business logic

### Models

- **EcbServerMessageException** - exception of this type is to be used when ECB API returns message - other than xml data payload - which then can be passed in exception message and in turn to the user / client.

- **ExchangeRateDto** - based on xml response from ECB

### Repositories

Implementations of interfaces

- **CacheRecordRepository Update()** is supposed to handle cases when there is given period of time and multiple CacheRecords spanning fragments of this period so they need to be merged into one if exchange rates are completely pulled for the given full period.

### Services

Interfaces and implementations for them:

- **IEcbClient** responsible for pulling data from data source - ECB API - returns data as dto class for consumption in codebase
- **ISdmxXmlReader** responsible for converting data from ECB API - after some looking around I figured implementing it as it is will be faster ( in dev time ) than utilising SdmxSource ?
- **IExchangeRateService** with its ExchangeRateCacheService implementation which is coordinating caching, mapping and persistence logic because time. Idea is - it checks whether requested exchange rates are in the database ( through *ICacheRecordRepository Get()* ) - if yes then fetch them - in other case fetch them from data source through *IEcbClient* and update cache.

## Tests

### Infrastructure

Checking if *IEcbClient* implementation ( and by dependency also *ISdmxXmlReader* ) can fetch data from ECB API and present them in correct form.

### Api

"Load tests" using k6 - basic scenarios - first time doing this anyway so I don't feel like I have expertise to know what to look for and what's relevant - in prepared scenarios I observed somewhat linear increase in request duration as more heavy tests are run.

- 1vus - <4ms
- 10vus - ~8ms
- 50vus - ~15ms
- 100vus - ~30ms
- 250vus - ~66ms

**Single requests at a time**
- when in favorable caching window:
  <3ms
- out of window:
  ~30ms
- ecb: 
  38-45ms

**Known issues and notes**

Mixed concurrent and sequential code - most likely places where performance and readability could be improved.

When running load test on fresh database concurrent conflicting inserts are running - lots of errors but it doesn't stop the server and data and server is fine after all.

Many first attempts at - sdmx, k6, caching etc. . Not particurarly pleased by looking at results given that they're from calling local server but maybe it's good enough - not sure - I had an idea and I went with it, learned some things, would hopefully do better next time. I have not given myself a lot of time too I think, so stuff was mostly rushed in ( explains issues above ) but I didn't want to commit too much - life and other stuff to do. Also it was supposed to be "proof of concept" so that's something I kept in mind and so I guess I was more inclined to do things quick ...
