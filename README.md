# CalculatorMarkelov

Практическая работа №2 — программа **«Калькулятор»** с использованием WPF, библиотеки классов и модульных тестов.

**Студент:** Маркелов Игорь Вячеславович  
**Группа:** ИПО-21

---

## Описание

Приложение реализует вычисление полных арифметических выражений, а не только одной операции над двумя числами. Поддерживаются `+`, `-`, `*`, `/`, `^`, скобки, несколько операций подряд и унарный минус.

Например, калькулятор корректно обрабатывает выражения `2+3*4`, `(2+3)*4`, `-2^2+6` и `2^(-2)`.

Архитектура разделена на три проекта:

- **CalculatorMarkelov.Core** — библиотека классов с логикой вычислений.
- **CalculatorMarkelov.Wpf** — WPF-приложение с графическим интерфейсом.
- **CalculatorMarkelov.Tests** — модульные тесты (MSTest).

---

## Структура проекта

```text
CalculatorMarkelov/
├── CalculatorMarkelov.sln
├── CalculatorMarkelov.Core/
│   ├── CalculatorMarkelov.Core.csproj
│   └── CalculatorEngine.cs
├── CalculatorMarkelov.Wpf/
│   ├── CalculatorMarkelov.Wpf.csproj
│   ├── App.xaml / App.xaml.cs
│   └── MainWindow.xaml / MainWindow.xaml.cs
├── CalculatorMarkelov.Tests/
│   ├── CalculatorMarkelov.Tests.csproj
│   └── CalculatorEngineTests.cs
└── docs/
    ├── report.md
    └── images/
```

---

## Используемые технологии

- **Microsoft Visual Studio**
- **C#**
- **.NET 9**
- **WPF**
- **MSTest**
- **Test Explorer**

---

## Сборка

1. Открыть `CalculatorMarkelov.sln` в Visual Studio.
2. `Build` → `Build Solution` (или `Ctrl + Shift + B`).

---

## Запуск

1. Установить `CalculatorMarkelov.Wpf` как Startup Project.
2. Нажать `F5`.

---

## Запуск тестов

1. `Test` → `Test Explorer`.
2. Нажать `Run All`.

Ожидаемый результат: **54 теста, 54 пройдены**.

---

## Сопроводительная записка

Готовая сопроводительная записка в формате PDF находится в файле `docs/report.pdf`.

Исходный markdown-файл записки находится в `docs/report.md`, а иллюстрации — в `docs/images/`.