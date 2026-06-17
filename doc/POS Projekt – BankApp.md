# POS Projekt – BankApp

**Klasse:** 3HIF · **Schuljahr:** 2025/26  
**Team:** Alexei & Chiara

---

## Inhaltsverzeichnis

1. [Projektbeschreibung](#1-projektbeschreibung)
2. [Softwarevoraussetzungen](#2-softwarevoraussetzungen)
3. [Projektplanung / Lastenheft](#3-projektplanung--lastenheft)
4. [Architektur](#4-architektur)
5. [Funktionsblöcke](#5-funktionsblöcke)
6. [Detaillierte Beschreibung der Umsetzung](#6-detaillierte-beschreibung-der-umsetzung)
7. [Mögliche Probleme und ihre Lösung](#7-mögliche-probleme-und-ihre-lösung)
8. [KI-Verwendung & Reflexion](#8-ki-verwendung--reflexion)
9. [Projekttagebuch](#9-projekttagebuch)

---

## 1. Projektbeschreibung

Die BankApp ist eine grafische Desktop-Anwendung (WPF, C#) zur Verwaltung einer fiktiven Bank. Sie kommuniziert über HTTP mit einem Python-FastAPI-Backend und unterstützt zwei Benutzerrollen:

- **Client:** Kann sich anmelden, eigene Karten und Kontostände einsehen, Überweisungen tätigen und Sparzinskonten eröffnen bzw. auszahlen lassen.
- **Manager:** Hat vollständigen Zugriff auf alle Kunden, Karten und Transaktionen. Kann Kunden anlegen/bearbeiten/löschen, Karten verwalten, Statistiken einsehen und den Audit-Log abrufen.

---

## 2. Softwarevoraussetzungen

### Entwicklung

| Software | Version |
|---|---|
| Windows | 10 / 11 (64-bit) |
| Visual Studio 2022 | ≥ 17.10 (für `.slnx`-Format) |
| .NET SDK | 10.0 |
| Git | beliebig |

### Laufzeit (Endnutzer)

| Komponente | Version |
|---|---|
| Windows | 10 / 11 (64-bit) |
| .NET Desktop Runtime | 10.0 |

### NuGet-Pakete

| Paket | Version | Zweck |
|---|---|---|
| `CommunityToolkit.Mvvm` | 8.4.2 | RelayCommand, AsyncRelayCommand |
| `LiveChartsCore.SkiaSharpView.WPF` | 2.0.0-rc4.5 | Diagramme in der Manager-Statistikansicht |
| `Serilog` | 4.3.1 | Logging |
| `Serilog.Sinks.Console` | 6.1.1 | Log-Ausgabe auf Konsole |
| `Serilog.Sinks.File` | 7.0.0 | Log-Ausgabe in Datei |
| `Microsoft.Extensions.DependencyInjection` | 10.0.8 | DI-Container |
| `xunit` | 2.x | Unit-Tests |
| `xunit.runner.visualstudio` | 2.x | Test-Runner in VS |

---

## 3. Projektplanung / Lastenheft

### 3.1 Must-Have Features

| Feature | Beschreibung |
|---|---|
| Login / Logout | JWT-basierte Authentifizierung, automatischer Logout bei abgelaufenem Token (401) |
| Kartenübersicht (Client) | Eigene Karten mit Guthaben, IBAN, Status anzeigen |
| Überweisung (Client) | Geld von eigener Karte an beliebige IBAN senden |
| Transaktionsverlauf (Client) | Eigene ein- und ausgehende Überweisungen einsehen |
| Sparzinsen (Client) | Sparkonto eröffnen, Zinsen nach Zeit auszahlen lassen |
| Kunden-CRUD (Manager) | Kunden anlegen (inkl. Rolle), bearbeiten, löschen |
| Kartenverwaltung (Manager) | Karten anlegen, löschen; Karten eines Kunden einsehen |
| Transaktionen (Manager) | Alle oder per-Kunde-Transaktionen einsehen |
| Statistiken (Manager) | Transaktionsanzahl und Volumen pro Konto als Tabelle + Diagramm |
| Audit-Log (Manager) | Alle Datenbankänderungen chronologisch abrufen |
| Sparzinsverwaltung (Manager) | Alle Sparzinseinlagen einsehen |
| Eigenes Profil bearbeiten | Jeder Benutzer kann eigene Daten inkl. Passwort aktualisieren |
| Logging | Serilog — alle Service-Aufrufe, Fehler und wichtige Events in Datei + Konsole |
| xUnit Tests | Modelltests für Berechnungslogik und Formatierungen |
| Mindestens 3 Fenster/Pages | Login, AccountView, TransactionView, MyInterests, Manager + Unterpanels |
| Interfaces | `IAccountService`, `ICardService`, `ITransactionService`, `IInterestService`, `IStatsService` |
| Abstrakte Klassen / Vererbung | `CommandBase` (abstract), alle ViewModels erben von `ViewModelBase` |

### 3.2 Nice-to-Have Features

| Feature | Status |
|---|---|
| Live-Diagramme (LiveCharts2) in Statistikansicht | ✅ umgesetzt |
| Automatischer Logout bei abgelaufenem JWT (401-Handler) | ✅ umgesetzt |
| Karten sofort aktualisieren nach Zinsen/Überweisungen | ✅ umgesetzt |
| Manager kann andere Manager anlegen | ✅ umgesetzt |
| Detaillierte Wertebereiche in Swagger / OpenAPI | ✅ umgesetzt |

---

## 4. Architektur

### 4.1 MVVM-Pattern

Die Anwendung folgt dem **Model-View-ViewModel**-Muster:

```
View (.xaml)          ←→   ViewModel (.cs)   ←→   Service (.cs)   ←→   REST-API
(UI-Darstellung)           (Logik, Commands)       (HTTP-Aufrufe)
```

- **Views** binden direkt an ViewModel-Properties (keine Code-Behind-Logik außer Dialog-Events).
- **ViewModels** erben von `ViewModelBase` und implementieren `INotifyPropertyChanged`.
- **Services** kapseln alle HTTP-Aufrufe und werden über `AppServices` (Singleton-Container) bereitgestellt.
- **Navigation** erfolgt über `NavigationStore` + `NavigationService`: jede Seite ist ein ViewModel, `MainWindow` zeigt das aktuelle ViewModel via `ContentControl` + `DataTemplate`.

### 4.2 Ordnerstruktur

```
BankApp/
├── Commands/
│   ├── CommandBase.cs          # Abstrakte Basisklasse für ICommand
│   └── NavigateCommand.cs      # Navigation via NavigationService
├── Models/
│   ├── AccountModel.cs
│   ├── CardModel.cs
│   ├── TransactionModel.cs
│   ├── InterestModel.cs
│   ├── BankModel.cs
│   ├── StatsModel.cs
│   └── AuditLogModel.cs
├── Services/
│   ├── Interfaces/             # IAccountService, ICardService, …
│   ├── AccountService.cs
│   ├── CardService.cs
│   ├── TransactionService.cs
│   ├── InterestService.cs
│   ├── BankService.cs
│   ├── StatsService.cs
│   ├── AppServices.cs          # Singleton-DI-Container
│   └── UnauthorizedHandler.cs  # DelegatingHandler für 401-Logout
├── Stores/
│   └── NavigationStore.cs
├── ViewModels/
│   ├── ViewModelBase.cs
│   ├── LoginViewModel.cs
│   ├── AccountViewModel.cs
│   ├── CardViewModel.cs
│   ├── TransactionViewModel.cs
│   ├── MyTransactionsViewModel.cs
│   ├── MyInterestsViewModel.cs
│   ├── ManagerViewModel.cs
│   └── ManagerInterestsViewModel.cs
├── Views/
│   ├── LoginView.xaml
│   ├── AccountView.xaml
│   ├── CardView.xaml
│   ├── TransactionView.xaml
│   ├── ClientTransactionView.xaml
│   ├── MyInterestsView.xaml
│   ├── ManagerView.xaml
│   ├── ManagerCardsView.xaml
│   ├── ManagerInterestsView.xaml
│   ├── AddClientDialog.xaml
│   ├── EditClientDialog.xaml
│   ├── AddCardDialog.xaml
│   ├── CreateInterestDialog.xaml
│   ├── EditProfileDialog.xaml
│   └── AmountPicker.xaml
└── BankApp.Tests/
    └── ModelTests.cs           # 12 xUnit-Tests
```

### 4.3 Navigationsfluss

```
LoginView
  └─ (Client-Login)  → AccountView
       ├─ Neue Überweisung  → TransactionView → AccountView
       ├─ Überweisungen     → ClientTransactionView → AccountView
       └─ Sparzinsen        → MyInterestsView → AccountView

  └─ (Manager-Login) → ManagerView
       ├─ Panel: Clients (Standard)
       ├─ Panel: Überweisungen
       ├─ Panel: Statistiken (Tabelle + Diagramme)
       ├─ Panel: Audit-Log
       └─ Seitenleiste: Sparzinsen → ManagerInterestsView → ManagerView
```

---

## 5. Funktionsblöcke

### 5.1 Authentifizierung & Session

- `LoginViewModel` ruft `AccountService.LoginAsync()` auf → JWT wird im `HttpClient`-Header gespeichert.
- `UnauthorizedHandler` (DelegatingHandler) überwacht alle HTTP-Antworten: Bei `401` wird ein statisches Event gefeuert, das in `App.xaml.cs` abonniert ist und über `Dispatcher.Invoke` den Logout + Navigation zur LoginView auslöst.

### 5.2 Client-Ansicht

- `AccountViewModel` lädt beim Start immer frische Karten via `CardService.GetCardsAsync()` (nicht aus dem gecachten Login-Objekt), damit nach Zinsen oder Überweisungen das Guthaben sofort aktuell ist.
- Jede Karte wird als `CardViewModel` dargestellt und zeigt Status, IBAN, Guthaben und einen Button für Neue Überweisung.

### 5.3 Manager-Ansicht (4-Panel-System)

Der `ManagerViewModel` verwendet ein `ManagerActiveView`-Enum mit 4 Werten (`Clients`, `Transactions`, `Stats`, `AuditLogs`). Jedes Panel wird via `DataTrigger` ein-/ausgeblendet. Die Sidebar hat Buttons für alle Panels sowie für CRUD-Aktionen auf Kunden und Karten.

### 5.4 Statistiken mit Live-Diagrammen

`LoadStats()` lädt Daten von `GET /stats/transactions-per-account` und befüllt:
- Eine `ObservableCollection<StatsModel>` für die Ranglisten-Tabelle
- `ISeries[] TxCountSeries` (Transaktionsanzahl) und `ISeries[] VolumeSeries` (Gesamtvolumen in €) für LiveCharts2-`CartesianChart`-Elemente

### 5.5 Logging

Serilog wird in `App.xaml.cs` konfiguriert (Konsole + rollierende Datei `log.txt`). Alle Service-Methoden loggen:
- `Log.Information(...)` bei Erfolg
- `Log.Warning(...)` bei HTTP-Fehler (nicht 2xx)
- `Log.Error(...)` bei Netzwerkfehler (`HttpRequestException`)

### 5.6 Unit-Tests

`BankApp.Tests/ModelTests.cs` enthält 12 xUnit-Tests für Modell-Berechnungen und Formatierungen (`DisplayBalance`, `DisplayTotal`, `DisplayRate`, `DisplayTimestamp`, `AmountEuros`, `CardStatusText`, `BankHeader`, etc.).

---

## 6. Detaillierte Beschreibung der Umsetzung

### 6.1 HTTP-Kommunikation

Alle HTTP-Aufrufe laufen über einen einzigen `HttpClient` in `AppServices`. Dieser wird beim App-Start mit `UnauthorizedHandler → HttpClientHandler` als Pipeline aufgebaut. Der Bearer-Token wird nach dem Login als Default-Header gesetzt und bei Logout entfernt.

### 6.2 Validierungen

Dialoge (Add/Edit Client, Add Card) validieren Eingaben clientseitig vor dem API-Aufruf. Fehler werden als roter `TextBlock` unterhalb des Feldes eingeblendet. Das Dialog-Fenster verwendet `SizeToContent="Height"` + `StackPanel`, damit Fehlermeldungen den Button nicht aus dem Fenster schieben.

### 6.3 Parallele Programmierung

In `ManagerViewModel.LoadClients()` werden Karten aller Kunden parallel geladen:

```csharp
// prompt: Lade alle Karten aller Kunden parallel mit Task.WhenAll
var cardTasks = Clients.Select(c => _services.CardService.GetCardsByAccountIdAsync(c.Id));
var results   = await Task.WhenAll(cardTasks);
```

Dies reduziert die Ladezeit bei vielen Kunden erheblich im Vergleich zu sequenziellem Laden.

### 6.4 Abstrakte Klassen & Vererbung

```
ICommand
  └── CommandBase (abstract)      ← abstrakte Basis, Execute ist abstract
        └── NavigateCommand       ← konkrete Implementierung

INotifyPropertyChanged
  └── ViewModelBase               ← gemeinsame Basis für alle ViewModels
        ├── LoginViewModel
        ├── AccountViewModel
        ├── ManagerViewModel
        └── ...
```

### 6.5 Interfaces

Alle Services implementieren ein Interface aus `Services/Interfaces/`:

| Interface | Implementierung |
|---|---|
| `IAccountService` | `AccountService` |
| `ICardService` | `CardService` |
| `ITransactionService` | `TransactionService` |
| `IInterestService` | `InterestService` |
| `IStatsService` | `StatsService` |

---

## 7. Mögliche Probleme und ihre Lösung

### Problem 1: 401-Logout im Hintergrund-Thread

**Problem:** `UnauthorizedHandler` wird auf einem Background-Thread aufgerufen. `MessageBox.Show()` und Navigation sind UI-Operationen und werfen eine `InvalidOperationException` wenn sie nicht auf dem UI-Thread ausgeführt werden.

**Lösung:** In `App.xaml.cs` wird das `Unauthorized`-Event mit `Dispatcher.Invoke(...)` abonniert, damit alle UI-Operationen garantiert auf dem UI-Thread laufen.

```csharp
UnauthorizedHandler.Unauthorized += () => {
    Dispatcher.Invoke(() => {
        services.AccountService.Logout();
        MessageBox.Show("Ihre Sitzung ist abgelaufen...");
        _navigationStore.CurrentViewModel = new LoginViewModel(services);
    });
};
```

---

### Problem 2: 401 beim Öffnen der Sparzinsen im Manager

**Problem:** `GET /cards/all/` (mit trailing slash) löste eine 307-Weiterleitung von FastAPI aus. .NET `HttpClient` entfernt den `Authorization`-Header bei Weiterleitungen. Das Backend sieht einen Aufruf ohne Token und antwortet mit `401` → Logout wird ausgelöst.

**Lösung:** Trailing Slash entfernt: `"/cards/all"` statt `"/cards/all/"`. Generelle Regel: FastAPI-Routen immer ohne abschließenden Slash aufrufen.

---

### Problem 3: Dialog-Buttons verschwinden bei Fehlermeldungen

**Problem:** Der `AddCardDialog` hatte eine feste `Height="480"` mit Grid-Rows `Height="*"`. Wenn Fehler-TextBlocks eingeblendet wurden, schoben sie die Buttons unter den sichtbaren Bereich.

**Lösung:** `Height` entfernt, `SizeToContent="Height"` gesetzt und das Layout von `Grid` auf `StackPanel` umgestellt. Das Fenster wächst nun dynamisch mit dem Inhalt.

---

### Problem 4: GridView-Spalten werden nicht angezeigt

**Problem:** Ein globaler `ListViewItem`-Style mit `ContentPresenter` im Template bricht die `GridView`-Darstellung, da GridView intern `GridViewRowPresenter` benötigt.

**Lösung:** In jedem `ListView` mit `GridView` wird ein lokaler `ListViewItem`-Style (im `ListView.Resources`) definiert, der explizit `GridViewRowPresenter` verwendet.

---

### Problem 5: Kartenguthaben veraltet nach Überweisung/Zinsen

**Problem:** Nach einer Überweisung oder Zinsauszahlung wurde zur `AccountView` zurücknavigiert, aber die Karten zeigten noch das alte Guthaben, weil die `AccountViewModel`-Konstruktion die gecachten `account.Cards` vom Login verwendete.

**Lösung:** `AccountViewModel` ruft immer `CardService.GetCardsAsync()` in `LoadCardsAsync()` auf und aktualisiert `account.Cards` frisch aus der API, unabhängig vom übergebenen Account-Objekt.

---

### Problem 6: `.slnx`-Format unbekannt

**Problem:** Das neue JSON-basierte Solution-Format `.slnx` (.NET 9+) wurde von älteren Visual Studio Versionen nicht erkannt.

**Lösung:** Visual Studio 2022 ≥ 17.10 verwenden. Das Format hat keine GUIDs und ist lesbarer als das klassische `.sln`.

---

## 8. KI-Verwendung & Reflexion

### 8.1 Eingesetzte Tools

| Tool | Verwendung |
|---|---|
| Claude Code (claude-sonnet-4-6) | Code-Generierung, Debugging, Refactoring, Dokumentation |

### 8.2 Was wurde mit KI verbessert / erstellt?

- Vollständige Umstrukturierung der Manager-Ansicht auf ein 4-Panel-System mit `ManagerActiveView`-Enum
- `UnauthorizedHandler` (DelegatingHandler) für automatischen 401-Logout
- Statistik-Panels mit LiveCharts2-Diagrammen
- Backend-Dokumentation (Swagger `summary`/`description`, Wertebereiche, Beispiele in `schemas.py`)
- Unit-Tests (`BankApp.Tests/ModelTests.cs`)
- Logging in allen Service-Klassen
- Diese Dokumentation

### 8.3 KI-Kennzeichnung im Code

Methoden und Algorithmen die mit KI erstellt wurden sind im Code mit `// prompt: ...` gekennzeichnet, z. B.:

```csharp
// prompt: Lade alle Karten aller Kunden parallel mit Task.WhenAll
var cardTasks = Clients.Select(c => _services.CardService.GetCardsByAccountIdAsync(c.Id));
var results   = await Task.WhenAll(cardTasks);
```

### 8.4 Reflexionskapitel

**Wo war KI hilfreich?**  
KI war besonders hilfreich bei der Fehleranalyse subtiler Probleme (trailing-slash-Bug, 401-Thread-Problem, GridView-Darstellung). Auch das schnelle Erstellen von Boilerplate-Code (Interfaces, Service-Implementierungen, XAML-Layouts) war deutlich schneller als von Hand.

**Was würden wir nächstes Mal anders machen?**  
Den Code-Prompt spezifischer formulieren, damit die KI weniger Annahmen treffen muss. Außerdem früher mit der Dokumentation beginnen und nicht alles am Ende nachliefern.

**Was hat gut/schlecht funktioniert?**  
Gut: Debugging und Erklärungen von Fehlern. Die KI hat oft die Ursache auf Anhieb erkannt.  
Schlecht: Bei sehr großen Kontexten (viele Dateien gleichzeitig) kamen manchmal inkonsistente Vorschläge. Es war wichtig, die KI auf konkrete Dateien zu fokussieren.

---

## 9. Projekttagebuch

| Datum | Was wurde gemacht | Wer |
|---|---|---|
| 15.05.2026 | Projektidee festgelegt, Anforderungen besprochen, Technologiestack gewählt (WPF, FastAPI) | Alexei & Chiara |
| 19.05.2026 | Klassendiagramme erstellt (ViewModels, Services, Models), GUI-Skizzen gezeichnet | Alexei & Chiara |
| 20.05.2026 | WPF-Projektstruktur angelegt, MVVM-Grundgerüst (ViewModelBase, NavigationStore, NavigationService) | Alexei |
| 21.05.2026 | Models angelegt (AccountModel, CardModel, TransactionModel), LoginView erstellt | Alexei |
| 22.05.2026 | AccountService mit Login/Logout, JWT-Token-Handling implementiert | Alexei |
| 26.05.2026 | AccountView und CardView erstellt, Kartendarstellung mit Guthaben und Status | Alexei |
| 27.05.2026 | TransactionView mit AmountPicker, Überweisung an beliebige IBAN | Alexei |
| 28.05.2026 | CardService (CRUD), ManagerViewModel Grundgerüst, Kunden-Liste | Alexei |
| 02.06.2026 | AddClientDialog, EditClientDialog, AddCardDialog mit Validierungen | Alexei |
| 03.06.2026 | **Zwischenpräsentation** — Prototyp vorgestellt | Alexei & Chiara |
| 04.06.2026 | Manager-Ansicht überarbeitet: 4-Panel-System mit ManagerActiveView-Enum | Alexei |
| 05.06.2026 | UnauthorizedHandler implementiert, automatischer Logout bei 401 | Alexei |
| 09.06.2026 | Trailing-Slash-Bug bei /cards/all gefunden und behoben | Alexei |
| 10.06.2026 | MyInterestsView, CreateInterestDialog, Sparzinsen-Auszahlung | Alexei |
| 10.06.2026 | ManagerInterestsView, Sparzinsen für Manager | Alexei |
| 11.06.2026 | StatsService, Statistik-Panel mit Ranglisten-Tabelle | Alexei |
| 12.06.2026 | LiveCharts2 integriert, zwei Diagramme in Statistik-Panel | Alexei |
| 13.06.2026 | AuditLog-Panel im Manager, AuditLogModel, StatsModel | Alexei |
| 13.06.2026 | EditProfileDialog, eigenes Profil bearbeiten (PUT /accounts/me) | Alexei |
| 16.06.2026 | Logging mit Serilog in alle Service-Klassen integriert | Alexei |
| 16.06.2026 | xUnit-Tests: 12 Modell-Tests, BankApp.slnx Solution-Datei | Alexei |
| 16.06.2026 | Manager kann jetzt Rolle beim Erstellen auswählen (Client/Manager) | Alexei |
| 16.06.2026 | Karten werden nach Zinsen/Überweisungen sofort aktualisiert | Alexei |
| 17.06.2026 | **Endpräsentation** — Abgabe | Alexei & Chiara |
