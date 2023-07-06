# TANKS_TEST_01_07_23
На выполнение ушло 5 полных дней.

# MAIN:
- Архитектура схожа с архитектурами DOD (Data-Oriented Design).
- Dependency Injection через VContainer.
- Практически весь код является процедурным (максимально последовательным) с помощью async (UniTask).
- Практически все данные игры являются конфигурацией и легко настраиваемы.
- Процедурная генерация уровней (Tilemap) по конфигурации.
- Искусственный интеллект реализован через стейты и Pathfinder A*.

# Дополнительно:
- Конфигурация представлена в виде Scriptable Objects.
- Используется новая система ввода.

# С какими проблемами столкнулся?
  Поскольку данные и системы разделены, то при удалении данных системы выходят из строя.
Это является проблемой, которую необходимо помнить. Поэтому большинство систем не привязано к определенному объекту,
но те, которые привязаны, пришлось использовать обойти токенами и Dispose. Это было не сильно приятно.

# Чтобы ещё доделал?
- Генерация уровней может падать при определенных параметрах, исправил бы более контролируемыми параметрами и формклами в коде.
- Поменял бы логику засева* карты преградами, чтобы ломаемые преграды были болеее выжными в логике. Я хотел бы увидеть результат, в котором примерно 50% преград ломаемые, а 50% — неломаемые.
- Также улучшил бы алгоритм создания стенок, скорее всего со стенками легче сначало их всех поставить а потом резать, в коде реализован алгоритм создания кусков, и он не сильно подходит.
- Было бы неплохо сделать конфигурацию для генерации уровней случайной, чтобы она была разной при каждом запуске сессии, для этого бы создал интерфейс провайдера конфигурациии, и сделал бы прокси провайдер с рандомизацией.
- Добавил бы новые механики в игру, о которых упоминалось в doc. Также бы изменил графику, добавив URP/HDRP.
- Отрефакторил главные стейты игры и инкапсулировал бы данные с сервисами для удобной манипуляции и уничтожения.
- В сцене валятся подконтрольные пулом объекты, я бы прокидывал условные контейнеры сбора.
