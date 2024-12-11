# Nuget-пакеты QA.DotNetCore.\*

## Назначение

Набор nuget-пакетов, реализующих Виджетную платформу (ВП) на ASP.NET Core, а также вспомогательные функции (например, кэширование):

| Название | Репозиторий | Описание |
|----------|-------------|----------|
| QA.DotNetCore.Caching | [QA nuget](https://nuget.qsupport.ru/packages/QA.DotNetCore.Caching) | Интерфейс и реализация провайдера кэширования для одного экземпляра ВП |
| QA.DotNetCore.Caching.Distributed | [QA nuget](https://nuget.qsupport.ru/packages/QA.DotNetCore.Caching.Distributed) | Реализация кэширования     поддерживающее несколько экземпляров ВП (с синхронизацией через Redis) |
| QA.DotNetCore.Engine.Abstractions | [QA nuget](https://nuget.qsupport.ru/packages/QA.DotNetCore.Engine.Abstractions) | Интерфейсы и абстрактные классы  для ВП
| QA.DotNetCore.Engine.AbTesting | [QA nuget](https://nuget.qsupport.ru/packages/QA.DotNetCore.Engine.AbTesting) | Возможности A/B тестирования на ВП |
| QA.DotNetCore.Engine.AbTesting.Configuration | [QA nuget](https://nuget.qsupport.ru/packages/QA.DotNetCore.Engine.AbTesting.Configuration) | Простое конфигурирование A/B тестирования на ВП |
| QA.DotNetCore.Engine.CacheTags | [QA nuget](https://nuget.qsupport.ru/packages/QA.DotNetCore.Engine.CacheTags) | Инфраструктура поддежрки кэш-тегов в ВП |
| QA.DotNetCore.Engine.CacheTags.Configuration | [QA nuget](https://nuget.qsupport.ru/packages/QA.DotNetCore.Engine.CacheTags.Configuration) | Конфигурирование сервисов кэш-тегов |
| QA.DotNetCore.Engine.OnScreen | [QA nuget](https://nuget.qsupport.ru/packages/QA.DotNetCore.Engine.OnScreen) |  Интеграция с админкой OnScreen |
| QA.DotNetCore.Engine.Persistent.Configuration | [QA nuget](https://nuget.qsupport.ru/packages/QA.DotNetCore.Engine.Persistent.Configuration) | Конфигурирование абстракций для доступа к данным |
| QA.DotNetCore.Engine.Persistent.Dapper | [QA nuget](https://nuget.qsupport.ru/packages/QA.DotNetCore.Engine.Persistent.Dapper) | Доступ к данным структуры сайта через Dapper |
| QA.DotNetCore.Engine.Persistent.Interfaces | [QA nuget](https://nuget.qsupport.ru/packages/QA.DotNetCore.Engine.Persistent.Interfaces) | Интерфейсы доступа к данным структуры сайта |
| QA.DotNetCore.Engine.QpData | [QA nuget](https://nuget.qsupport.ru/packages/QA.DotNetCore.Engine.QpData) | Реализация структуры сайта на основе QP |
| QA.DotNetCore.Engine.QpData.Configuration | [QA nuget](https://nuget.qsupport.ru/packages/QA.DotNetCore.Engine.QpData.Configuration) | Простое конфигурирование структуры сайта на основе QP |
| QA.DotNetCore.Engine.Reflection | [QA nuget](https://nuget.qsupport.ru/packages/QA.DotNetCore.Engine.Reflection) | реализация ITypeFinder для .NET Core |
| QA.DotNetCore.Engine.Routing | [QA nuget](https://nuget.qsupport.ru/packages/QA.DotNetCore.Engine.Routing)| роутинг для ВП |
| QA.DotNetCore.Engine.Targeting | [QA nuget](https://nuget.qsupport.ru/packages/QA.DotNetCore.Engine.Targeting) | таргетирование для ВП |
| QA.DotNetCore.Engine.Widgets | [QA nuget](https://nuget.qsupport.ru/packages/QA.DotNetCore.Engine.Widgets) | серверный рендеринг виджетов |
| QA.DotNetCore.Engine.Xml | [QA nuget](https://nuget.qsupport.ru/packages/QA.DotNetCore.Engine.Xml) | Реализация структуры сайта на основе XML |
| QA.DotNetCore.Engine.Xml.Configuration | [QA nuget](https://nuget.qsupport.ru/packages/QA.DotNetCore.Engine.Xml.Configuration) | Простое конфигурирование структуры сайта на основе XML |

## История версий

### 3.4.0

* Переход на .NET8

### 3.3.3

* Исправлена проблема лишних соединений для гранулярного кэша

### 3.3.2

* Исправлена логика блокировки инвалидации

### 3.3.1

* Исправлены версии библиотек в пакетах

### 3.3.0

* Исправлено несколько проблем с созданием лишних соединений с БД
* Добавлено расширенное логирование основных операций БД

### 3.2.27

* Исправлена инициализация по умолчанию для UseSiteStructure (обеспечение обратной совместимости)
* Добавлено дополнительное логирования для подключений к БД

### 3.2.26

* В UseSiteStructure добавлена возможность передачи экземляра ExcludePathChecker, чтобы иметь возможность исключать определённые пути в приложении из обработки виджетной платформой

### 3.2.25

* В методe ITargetingFilter GetFilter в ComponentExtensions поправлена проблема с производительностью фильтров таргетинга за счёт того, что WidgetFilter теперь выполняется первым и отсекает большую долю виджетов и страниц сразу, не применяя к ним другие более сложные и ресурсоёмкие фильтры.

### 3.2.24

* В метод GetUntypedFields в UniversalAbstractItem добавлена возможность возврата пустых значений полей (#175018)

### 3.2.23

* При загрузке связей M2M убрана загрузка архивных статей

### 3.2.22

* Библиотка Quantumart.AspNetCore обновлена до 6.0.15 (#174238)

### 3.2.21

* Запросы вместо United-таблиц теперь будут идти к Uniged-представлениям исчерпание коннекций (#173019)

### 3.2.20

* Исправлено потенциальное исчерпание коннекций (#173680)

### 3.2.19

* Вернулся `TargetingKeys` (был удален в 3.2.16)
* Секция `DictionariesConfig` теперь необязательная

### 3.2.18

* Добавлен метод `AddBasicCacheTagServices` для регистрации базовых сервисов, не завязанных на QP

### 3.2.17

* Ошибка инвалидации не должна приводить к перезапуску пода (#173041)

### 3.2.16

* Добавлены сервисы по работе со справочниками IAbstractItem
* Добавлены реализации фильтров таргетинга ITargetingFilter
  * RelationFilter - абстрактный фильтр для relation полей
  * ManyToManyFilter - абстрактный фильтр для m2m полей
  * OneToManyFilter - абстрактный фильтр для o2m полей
  * CultureFilter - фильтр по культуре
  * RegionFilter - фильтр по регионам
* Добавлены реализации провайдеров данных для фильтров ITargetingProvider
  * QueryTargetingProvider - провайдер данных query string
  * RelationTargetingProvider - абстрактный провайдер справочников IAbstractItem
  * CultureTargetingProvider - провайдер культур
  * RegionTargetingProvider - провайдер регионов
* Добавлен базовый интерфейс ITargetingProviderSource для провайдеров таргетинга
* Добавлен провайдер DictionariesPossibleValuesProvider для корректной работы url токенов в демосайте
* Удален интерфейс ITargetingFiltersFactory
* Добавлен интерфейс ITargetingContextUpdater с реализацией для возможности на выбор передавать таргетинг явно или через TargetingMiddleware
* Добавлен интерфейс ITargetingRegistration для регистрации фильтров и провайдеров, а также механизм регистрации из внешних сборок
* Добавлена реализация KeyedServiceSetConfigurator, которая теперь используется для регистрации фильтров таргетинга
* Удален проект с примером API для виджетной платформы
* Донастроен проект демосайта для корректной работы таргетинга

### 3.2.15

* Внутренняя версия

### 3.2.14

* Внутренняя версия

### 3.2.13

* Исправлен scope для QpContentCacheTracker

### 3.2.12

* Добавлен интерфейс ITargetingFiltersFactory

### 3.2.11

* Выполнены доработки для поддержки мультитенатного кэша:
  * Добавлено явное управление жизненным циклом UnitOfWork в QPCacheTracker
  * Генерация кэш-тегов в DefaultQpContentCacheTagNamingProvider теперь использует кастомер-коды
  * Удалён лишний параметр SiteId из запроса GetContentsById

### 3.2.10

* Исправлен возврат пустых M2M для статей, где нет ни одного значения M2M по всем полям

### 3.2.9

* Добавлен вызов Dispose для семафора при очистке

### 3.2.8

* Добавлен CleanCacheLockerService для очистки ключей объектов-блокировок и семафоров, которые не использовались некоторое время
* Перенесён из QA.Core метод GetOrAddBatch

### 3.2.7

* Удалены неиспользуемые настройки из RedisCacheSettings

### 3.2.6

* Исправлено возможное дублирование базовых URL

### 3.2.5

* Исправлен возможный NRE

### 3.2.4

* Исправлена ошибка инициализации SemaphoreAsyncLock

### 3.2.3

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

### 3.1.9

* Увеличены значения по умолчанию для интервалов кэширования

### 3.1.8

* Исправлена инвалидация тегов по таймеру
* Добавлена возможность настраивать интервал опроса для инвалидации

### 3.1.7

Удаление лишней зависимости

### 3.1.6

Исправление NRE

### 3.1.5

Расширение вспомогательных методов для регистрации DI

### 3.1.4

Обновление nuget-зависимостей

### 3.1.3

* Fix error on startup by consolidating versions of MassTransit dependencies

## Более старые версии

Описаны в файле [old_changes.md](old_changes.md)
