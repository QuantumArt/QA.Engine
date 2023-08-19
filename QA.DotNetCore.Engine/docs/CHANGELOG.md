# Changelog

## 3.2.9

* Добавлен вызов Dispose для семафора при очистке

## 3.2.8

* Добавлен CleanCacheLockerService для очистки ключей объектов-блокировок и семафоров, которые не использовались некоторое время
* Перенесён из QA.Core метод GetOrAddBatch

## 3.2.7

* Удалены неиспользуемые настройки из RedisCacheSettings

## 3.2.6

* Исправлено возможное дублирование базовых URL

## 3.2.5

* Исправлен возможный NRE

## 3.2.4

* Исправлена ошибка инициализации SemaphoreAsyncLock

## 3.2.3

* Исправлена передача настроек Redis в RedLock #172085 

## 3.2.2

* Исправлена ошибка с конкуррентным обновлением #171995 

## 3.2.1

* Добавлена поддержка Port для подключения MassTransit with RabbitMQ

## 3.2.0

* Исправлена ошибка с лишним возникновением DeprecateCacheIsExpiredOrMissingException
* Исправлена совместная работа гранулярного кэша и ленивой загрузки данных
* Переписана логика работы внешнего кэша (Redis):
  * Исправлена логика deprecated-значений на консистентную с in-memory
  * Исправлена логика работы с блокировками на консистентную с in-memory
  * Добавлено хранение информации во внешнем кэше, а не только инвалидация
  * Добавлено перезаполнение локального кэша по внешнему
  * Исправлено именование ключей, тегов, блокировок (устранены возможные конфликты)
  * Исправлены проблемы с сериализацией
  
## 3.1.13

* Оптимизированы некоторые SQL-запросы

## 3.1.12

* Добавлена поддержка полей FrontModuleName и FrontModuleUrl в ItemDefinition с поддержкой обратной совместимости (если поля отсутствуют).

## 3.1.11

* Убрана лишняя инвалидация на старте приложения
* Исправлен InvalidCastException при обращении к Details в UniversalAbstractItem

## 3.1.10

* Реализована возможность переключения режимов кэширования для структуры сайта
* Режим кэширования Simple для структуры сайта выбран по умолчанию
* Исправлена работа зависимых классов для режима кэширования Simple (решена проблема с количеством SQL-коннекций)

## 3.1.9

* Увеличены значения по умолчанию для интервалов кэширования

## 3.1.8

* Исправлена инвалидация тегов по таймеру
* Добавлена возможность настраивать интервал опроса для инвалидации

## 3.1.7

Удаление лишней зависимости

## 3.1.6

Исправление NRE

## 3.1.5

Расширение вспомогательных методов для регистрации DI

## 3.1.4

Обновление nuget-зависимостей

## 3.1.3

### Fix MassTransit error on startup

- Fix error on startup by consolidating versions of MassTransit dependencies

## 3.1.2

### Add changelog

## 3.1.1 ([48e6cb3](https://tfs.dev.qsupport.ru/tfs/QuantumartCollection/QA.Core/_git/QA.Engine/commit/48e6cb32abe381cd3af9764e43a76a766c0f1ab1))

### Improve structure of projects and DI

- Remove obsolete extension methods
- Move DI-related code to projects with `.Configuration` suffix (or separate folders)
- Rename project `QA.DotNetCore.Caching.HostedService` back to `QA.DotNetCore.Engine.CacheTags.Configuration` as it's more appropriate name
- Simplify DI using hierarchical initialization (DI of higher layers use DI of lower)

## 3.1.0-beta1 ([166104f](https://tfs.dev.qsupport.ru/tfs/QuantumartCollection/QA.Core/_git/QA.Engine/commit/166104f835e2d4d517ca257e92cbf74f4d7341d5))

### Добавлена инвалидация кэша по событию

- Support invalidation via RabbitMQ (MassTransit)

## 3.0.0 ([89179fa](https://tfs.dev.qsupport.ru/tfs/QuantumartCollection/QA.Core/_git/QA.Engine/commit/89179fa8acb8917d56619898ec220ddb5d0c8977))

### Auxiliary changes related to adding distributed cache tracking

- Reduce minimal cache expiration time to speed up tests
- Add tests
    - Add distributed memory integration tests
    - Add `RedisCacheProvider` unit tests
    - Define test traits (category, task)
    - Set timeout on tests in case of bad network connection or performance degradation
    - Extract helpers to separate project (`CommonUtils`)
- Fix misleading obsolete message (for `UseCacheTagsInvalidation`)
- Move distributed memory cache to distributed project
- Cosmetic change: Fix typos in comments and names
- Cosmetic change: parameter wrapping and pattern matching
- Adjust auxiliary tools configuration
    - Adjust rules in editor config
    - Add live unit test config
- Add .net 6.0 support for libraries
- Remove unused dependencies from projects
- Update dependencies

## 2.3.0 ([83585c9](https://tfs.dev.qsupport.ru/tfs/QuantumartCollection/QA.Core/_git/QA.Engine/commit/83585c9e664e92279cc3c0937639d535fd448581))

### Store invalidation state in redis to support distributed tracking

- :bug: Fix add cache operation in chained cache
    Pass data instead of tags as a value to cache
- Target Caching project to `.netstandard 2.1` to be able to use useful features (e.g. initialize `HashSet` with capacity to avoid rehashing)
- Fix application shutdown when redis is not available on start
    Retry to obtain unique app id if exception occurred
- Start invalidation check immediately after worker started
- Change method signatures to request specific types
    Change `Get` and `TryGetValue` to request specific types
    to be able to deserialize data (in redis cache provider case)
- :bug: Fix cache entities collision in `VersionedCacheCoreProvider`
    Fix collision between tags and values when stored in-memory. E.g. when set cache with key `Garlic` and tag `Garlic` an error had occurred.
- :bug: Invalidate distributed memory cache on set
    Invalidate all linked in-memory cache on cache values change (i.e. when setting new value for existing cache key)
- Extract modified storage abstraction from cache watcher
    - Extract modified storage abstraction from cache watcher to be able to use distributed storage
    - Refactoring of cache watcher
- Supported distributed modification storage to be able to store invalidation state in redis

## 2.2.1 ([192dfcd](https://tfs.dev.qsupport.ru/tfs/QuantumartCollection/QA.Core/_git/QA.Engine/commit/192dfcd1873483a773f70d61e09891270c01a812))

### Fix united filter

- Fix united filter
    Assign collection before premature exit of pipe

## 2.2.0 ([f111be3](https://tfs.dev.qsupport.ru/tfs/QuantumartCollection/QA.Core/_git/QA.Engine/commit/f111be3e2116798aba32f5122483ce40eab912cf))

### Performance optimization for warmed cache

- Compress before writing cache to redis
    Use gzip compression to reduce redis traffic (and IO operation duration)
- Omit properties null and default values on serialization
- Optimize bottlenecks
    - Allocate required capacity for dictionary in UntypedFields to avoid rehashing
    - Ignore properties with null value to remove duplicated filtering in wp consumers
    - Rewrite linq to loop
- Optimize string comparison
    Use ordinal instead of invariant culture to avoid most likely unnecessary culture-aware comparison overhead
- Avoid allocations of empty collections
    Replace allocations to static empty collections
- Codestyle fixes
    - Add missing modifiers to methods
    - Remove and sort namespaces
    - Fix spaces and wrappings

## 2.1.7 ([45716b4](https://tfs.dev.qsupport.ru/tfs/QuantumartCollection/QA.Core/_git/QA.Engine/commit/45716b4df75029d6257aced15f68844a6a80e417))

### Fix invalidation when changed only abstract item

- Fix invalidation when changed only abstract item

## 2.1.6 ([4b054bb](https://tfs.dev.qsupport.ru/tfs/QuantumartCollection/QA.Core/_git/QA.Engine/commit/4b054bb0a4663eb2e3c558fdecf912dc53fc5936))

### Fix cache invalidation

- Fix cache invalidation when new content is added. Caused by invalid tag names

## 2.1.5 ([11f197d](https://tfs.dev.qsupport.ru/tfs/QuantumartCollection/QA.Core/_git/QA.Engine/commit/11f197dfbd953b19db4d53cfe1c9812af20f63dc))

### Add perfomance logs for redis IO operation

- Add perfomance logs for redis IO operation

## 2.1.4 ([7401903](https://tfs.dev.qsupport.ru/tfs/QuantumartCollection/QA.Core/_git/QA.Engine/commit/7401903f19fa112502775b12085430de1a59f2d0))

### Shorten and fix some logging

- Fix logging
    - Add readable names for logged properties (instead of numbers)
    - Shorten and refine pipeline logs (remove uninformative message part)
- Cosmetic changes (auto cleanup)

## 2.1.3 ([0ba5126](https://tfs.dev.qsupport.ru/tfs/QuantumartCollection/QA.Core/_git/QA.Engine/commit/0ba5126dcc60a1671333c16f849c98a5b66db5a0))

### Исправление нагрузки на CPU

- Lock keys sequentially to avoid deadlocks
- Remove `Parallel.For` to avoid CPU pressure

## 2.1.2 ([d05b981](https://tfs.dev.qsupport.ru/tfs/QuantumartCollection/QA.Core/_git/QA.Engine/commit/d05b9819d2fb5be4b5ca739078f63302eeca5df0))

### Добавление пассивной распределенной блокировки при обращении через redis кэш к БД (GetOrAdd)

- Add some logging
- Fix pack key removal on tag invalidation
- Cosmetic changes
- Use milliseconds instead of seconds for redis cache
- Add exclusive lock for redis cache
    - Support deprecated cache
    - Add exclusive lock for redis cache
        Lock by key when requesting data from db in `GetOrAdd` (to reduce requests to db)
    - Add tests for concurrent execution of `GetOrAdd` method
    - Handle previously suppressed errors on transaction executions

## 2.1.1 ([44ecb7d](https://tfs.dev.qsupport.ru/tfs/QuantumartCollection/QA.Core/_git/QA.Engine/commit/44ecb7d2e0fd4bb2b4e912d46b1ff1dc59396bf1))

### Оптимизация построения AbstractItemStorage при инвалидации

- Add helpers to chain operations over collections
    Add `OperationChain` to encapsulate keys/results filtering after each operation (e.g. for several layers of caching)
- Optimize extensions relations initialization on cache invalidation
    - Use one query to db to obtain all extension relations
    - Cache extension relations and get only invalidated part of them
    - Rename `IsExist` to `Exist` (since it accepts multiple keys)
    - Change `Get`, `IsSet` and `GetOrAdd` to support multiple keys (to reduce number of requests to db and redis)
- Fix and add tests

## 2.1.0 ([27823b4](https://tfs.dev.qsupport.ru/tfs/QuantumartCollection/QA.Core/_git/QA.Engine/commit/27823b40da82b16cc8750332a91a00ffb9b98b02))

### Добавление поддержки распределенного кэширования

- Add redis cache with tag support
- Cover redis cache with integration tests
- Add asynchronous methods for redis cache
- Codestyle cleanup for `VersionedCacheCoreProvider`
- Make `AbTestContainersByPath` class serializable to be able to cache its instances
- Remove `AbstractItemStorage` caching
    Complexity on deserialization of tree with bidirectional relations is greater then building it from separate items (`AbstractItem`s)
- Extract invalidation cache hosted service to separate assembly to avoid aspnetcore runtime dependency in worker service

## 2.0.4 ([65bf9d6](https://tfs.dev.qsupport.ru/tfs/QuantumartCollection/QA.Core/_git/QA.Engine/commit/65bf9d6dbf8c0ac84071863b60b51630908cc0b4))

### Bump to stable version (region changes)

Bump to stable version (region changes)

## 2.0.4-beta2 ([c243c68](https://tfs.dev.qsupport.ru/tfs/QuantumartCollection/QA.Core/_git/QA.Engine/commit/c243c685b040f6c7d2ce8d70d40f41d8604f2739))

### Игнорирование регистра при получении полей-расширений у AbstractItem'а

- Ignore case for abstract items' untyped fields to optimize search by key and simplify usage
- Add region storage to be able to take into account ancestors of targeting region when filtering (e.g. filter `{ Moscow }` will be extended to `{ Moscow, Russia }`)
- Cache region storage building to avoid accessing db on each request

## 2.0.3 ([aefeb82](https://tfs.dev.qsupport.ru/tfs/QuantumartCollection/QA.Core/_git/QA.Engine/commit/aefeb82121978c73ea05fa16e24952b2de921e41))

### Изменение способа кэширования AbstractItemStorage

- Cache all operations using db on hot path
- Add abstractions for combined (memory and distributed) caching
    - Add memory cache provider for local caching
    - Add node identifier to be able to distinguish node (and its local cache) globally
- Use node-aware in-memory caching for abstract item storage
    Redis will not able to serialize it due to data diversity (variety of descendants of `IAbstractItem`). Since we are unable to serialize abstract item storage we store it in-memory and only check cache invalidation globally.

## 2.0.2 ([17fea96](https://tfs.dev.qsupport.ru/tfs/QuantumartCollection/QA.Core/_git/QA.Engine/commit/17fea96794e2c63da3270cb505dd328e476f8a1d))

### Fix invalid published version

- Build `2.0.1-beta2` published inconsistent versions for several packages. To fix it we should bump version of all packages to `2.0.2`

## 2.0.1 ([71034d0](https://tfs.dev.qsupport.ru/tfs/QuantumartCollection/QA.Core/_git/QA.Engine/commit/71034d03a25bd9b0cc0fb5ca2213452f4df9d0d6))

### Исправление долгих запросов от API виджетной платформы

- Optimize sql call on query construction
    - Use single sql call to obtain content metadata instead of multiple
    - Cache metadata db call
    - Ignore case for script tokens
    - Add separate method to extract contentNetNames from script template (to create cache tags for resulted query)
- Cache plain abstract items and item definitions to fix long requests on stage
- Fix tests after adding new dependencies

## 2.0.0-beta

- поддержка .NET5

## 1.5.11

- фикс проблемы с избыточным кол-вом открытий соединений с БД. если при обработке запроса не нужно реально идти в БД (из-за попадания в кеш, как правило), то соединение всё равно открывалось.

## 1.5.10

- добавилась опция CacheFetchTimeoutAbstractItemStorage при регистрации структуры сайта. Она является таймаутом ожидания получения структуры сайта из кеша (имеет смысл, когда база геоудалена от приложения, в таких случаях структура сайта может строиться долго, и "непрогретый" сайт спустя этот таймаут будет падать с DeprecateCacheIsExpiredOrMissingException).

## 1.5.9

- сделал настраиваемыми свойства куки, хранящую информацию об аутентификации onscreen: SameSiteMode (Lax по умолчанию), SecurePolicy (SameAsRequest по умолчанию)

## 1.5.8

- исправлен баг "Collection was modified" при роутинге в TailUrlResolver

## 1.5.7

- добавлен метод GetOrAddAsync для ICacheProvider (tfs #157535)

## 1.5.6

- добавлен общий механизм проверки строк по wildcard (tfs #157465)
- используется при подборе подходящей стартовой страницы, исходя из шаблонов их dnsbindings
- используется при проверках AllowedUrlPatterns/DeniedUrlPatterns в WidgetFilter

## 1.5.5

- добавлена возможность параметризовать зоны, задаваемые через RenderZonesInText (tfs #157350)
breaking change: изменил в сигнатуре метода WidgetZone тип аргумента arguments с object на IDictionary<string, object>

## 1.5.4

- добавлена нетипизированная загрузка полей AbstractItem в коллекцию Details для элементов структуры сайта без extension (tfs #157148)

## 1.5.3

- фикс ошибки в роутинге, когда существует и страница, и виджет с одним алиасом на одном уровне (tfs #157029)

## 1.5.2

- доработка шаблонов хвоста урла для endpoint routing: у TailUrlMatchingPattern появилась возможность задавать список возможных значений прямо в шаблоне (к примеру {action[Archive|Popular]}); также теперь можно задавать regex-ограничения с помощью свойства Constraints

## 1.5.1

- StartPageNotFoundException в случае, когда стартовая страница для запроса не найдена

## 1.5.0

- Поддержка .net core 3.1.
- Поддержка endpoint routing для структуры сайта, метод MapSiteStructureControllerRoute
- Поддержка endpoint routing для аб-тестов, метод MapAbtestEndpointRoute
- Изменение способа регистрации шаблонов урлов сайта, где может задаваться таргетирование, теперь это делается в опциях при настройке AddSiteStructureEngine

Breaking changes:

- Изменился способ регистрации сервисов через ServiceSetConfigurator, вместо RegisterSingleton и RegisterScoped просто Register (lifetime всё равно задается при регистрации в IoC)
- Из IStartPage убран метод GetUrlResolver
- Упразднён интерфейс ITargetingUrlResolver - разбит на ITargetingUrlTransformator и IHeadUrlResolver
- Упразднён класс TargetingUrlResolverFactory
- Интерфейс ITargetingContext больше не имеет метода GetPossibleValues
- Изменилась сигнатура метода app.UseTargeting, теперь там нельзя регистрировать провайдеры возможных значений, это нужно делать через метод RegisterUrlHeadTokenPossibleValues в опциях при настройке AddSiteStructureEngine
- ITargetingPossibleValuesProvider переименован в IHeadTokenPossibleValuesProvider

## 1.1.0

- Поддержка возможности построения структуры сайта без указания разработчиком классов для страниц и виджетов (метод service.AddSiteStructure - урезанная версия service.AddSiteStructureEngine), это полезно когда нужно просто получить какую-то информацию из структуры, а не строить на основе неё сайт.
- Теперь разработчику моделей страниц и виджетов не нужно указывать атрибуты LibraryUrl и LoadManyToManyRelations, виджетная платформа сама определяет типы полей.

## 1.0.9

- поддержка полей иконок blueprint для ItemDefinition.

## 1.0.8

- tag helper для разметки статей для режима onscreen; возможность задавать типы виджетов, которые не нужно показывать в onscreen

## 1.0.7

- Поддержка управления порядком виджетов в OnScreen на клиенте

## 1.0.6

- Исправлен resolving зависимостей для инвалидации кэша в stage-режиме.

## 1.0.5

- Добавлен ItemFinder для поиск по структуре сайта

## 1.0.4

- Исправлена инвалидация кэша

## 1.0.3

- Исправлено вычисление URL библиотеки контента для PG
