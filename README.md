# BankApp

**Klasse:** 3HIF · **Schuljahr:** 2025/26  
**Team:** Alexei & Chiara

Eine WPF-Desktop-Anwendung zur Verwaltung einer fiktiven Bank. Kommuniziert über HTTP mit einem [Python-FastAPI-Backend](https://github.com/GAlexus555/BankAppAPI) und unterstützt zwei Benutzerrollen.

---

## Rollen

| Rolle | Rechte |
|---|---|
| **Client** | Login, eigene Karten & Guthaben einsehen, Überweisungen tätigen, Sparzinskonten verwalten |
| **Manager** | Alle Kunden/Karten/Transaktionen verwalten, Statistiken & Audit-Log abrufen, neue Manager anlegen |

---

## Voraussetzungen

### Entwicklung

| Software | Version |
|---|---|
| Windows | 10 / 11 (64-bit) |
| Visual Studio 2022 | ≥ 17.10 (für `.slnx`-Format) |
| .NET SDK | 10.0 |

### Laufzeit (Endnutzer)

| Komponente | Version |
|---|---|
| Windows | 10 / 11 (64-bit) |
| .NET Desktop Runtime | 10.0 |

### NuGet-Pakete

| Paket | Version | Zweck |
|---|---|---|
| `CommunityToolkit.Mvvm` | 8.4.2 | RelayCommand, AsyncRelayCommand |
| `LiveChartsCore.SkiaSharpView.WPF` | 2.0.0-rc4.5 | Diagramme in der Statistikansicht |
| `Serilog` | 4.3.1 | Logging |
| `Serilog.Sinks.Console` | 6.1.1 | Log-Ausgabe auf Konsole |
| `Serilog.Sinks.File` | 7.0.0 | Log-Ausgabe in Datei |
| `Microsoft.Extensions.DependencyInjection` | 10.0.8 | DI-Container |
| `xunit` | 2.x | Unit-Tests |

---

## Architektur

Die Anwendung folgt dem **MVVM-Pattern**:

```
View (.xaml)  ←→  ViewModel (.cs)  ←→  Service (.cs)  ←→  REST-API
```

- **Views** binden direkt an ViewModel-Properties (kein Code-Behind außer Dialog-Events)
- **ViewModels** erben von `ViewModelBase` (`INotifyPropertyChanged`)
- **Services** kapseln alle HTTP-Aufrufe und sind über `AppServices` (Singleton-DI) erreichbar
- **Navigation** über `NavigationStore` + `NavigationService` — jede Seite ist ein ViewModel

### Ordnerstruktur

```
BankApp/
├── Commands/           # CommandBase (abstract), NavigateCommand
├── Models/             # AccountModel, CardModel, TransactionModel, InterestModel, …
├── Services/
│   ├── Interfaces/     # IAccountService, ICardService, ITransactionService, …
│   ├── AccountService.cs
│   ├── CardService.cs
│   ├── TransactionService.cs
│   ├── InterestService.cs
│   ├── StatsService.cs
│   ├── AppServices.cs          # Singleton-DI-Container
│   └── UnauthorizedHandler.cs  # Automatischer Logout bei 401
├── Stores/             # NavigationStore
├── ViewModels/         # LoginViewModel, AccountViewModel, ManagerViewModel, …
├── Views/              # LoginView, AccountView, ManagerView, Dialoge, …
└── BankApp.Tests/      # 12 xUnit-Tests
```

### Navigationsfluss

```
LoginView
  ├─ (Client)  → AccountView
  │               ├─ Überweisung  → TransactionView
  │               ├─ Verlauf      → ClientTransactionView
  │               └─ Sparzinsen   → MyInterestsView
  └─ (Manager) → ManagerView
                  ├─ Panel: Clients (Standard)
                  ├─ Panel: Überweisungen
                  ├─ Panel: Statistiken (Tabelle + Diagramme)
                  ├─ Panel: Audit-Log
                  └─ Sidebar: Sparzinsen → ManagerInterestsView
```

---

## Features

### Client
- JWT-Login / Logout
- Kartenübersicht mit aktuellem Guthaben (wird nach jeder Aktion sofort aktualisiert)
- Überweisung an beliebige IBAN
- Transaktionsverlauf
- Sparzinsen eröffnen & auszahlen lassen

### Manager
- Kunden anlegen (als Client oder Manager), bearbeiten, löschen
- Karten anlegen, löschen
- Transaktionen aller Kunden oder eines bestimmten Kunden anzeigen
- Statistikansicht: Tabelle + LiveCharts2-Diagramme (Transaktionsanzahl & Volumen pro Konto)
- Audit-Log aller Datenbankänderungen
- Alle Sparzinseinlagen einsehen

### Technisch
- Automatischer Logout bei abgelaufenem JWT (`UnauthorizedHandler` → `Dispatcher.Invoke`)
- Paralleles Laden von Kundenkarten via `Task.WhenAll`
- Serilog-Logging in alle Service-Klassen (Konsole + rollierende Datei `log.txt`)
- 12 xUnit-Unit-Tests für Modell-Berechnungen und Formatierungen

---

## Bekannte Probleme & Lösungen

| Problem | Lösung |
|---|---|
| 401-Logout im Hinterground-Thread wirft `InvalidOperationException` | `Dispatcher.Invoke(...)` in `App.xaml.cs` |
| Trailing Slash (`/cards/all/`) löst 307-Redirect aus → Authorization-Header geht verloren | Alle API-Aufrufe ohne abschließenden Slash |
| Dialog-Buttons verschwinden wenn Fehler-TextBlocks erscheinen | `SizeToContent="Height"` + `StackPanel` statt fixer `Height` |
| GridView-Spalten werden nicht angezeigt mit globalem `ListViewItem`-Style | Lokaler Style mit `GridViewRowPresenter` pro `ListView` |
| Kartenguthaben veraltet nach Überweisung/Zinsen | `AccountViewModel` lädt Karten immer frisch via `CardService.GetCardsAsync()` |

---

## KI-Verwendung

Folgende Teile wurden mit KI-Unterstützung (Claude Code) erstellt und sind im Code mit `// prompt: ...` gekennzeichnet:

- `ManagerViewModel.LoadClients()` — paralleles Laden mit `Task.WhenAll`
- `ManagerViewModel.LoadStats()` — LiveCharts2-Diagrammaufbau
- `InterestService` — vollständige Implementierung mit Serilog-Logging

---

## Projekttagebuch

| Datum | Was wurde gemacht | Wer |
|---|---|---|
| 15.05.2026 | Projektidee, Anforderungen, Technologiestack gewählt | Alexei & Chiara |
| 19.05.2026 | Klassendiagramme, GUI-Skizzen | Alexei & Chiara |
| 20.05.2026 | MVVM-Grundgerüst (ViewModelBase, NavigationStore) | Alexei |
| 21.05.2026 | Models, LoginView | Alexei |
| 22.05.2026 | AccountService mit JWT-Login | Alexei |
| 26.05.2026 | AccountView, CardView | Alexei |
| 27.05.2026 | TransactionView, AmountPicker | Alexei |
| 28.05.2026 | CardService, ManagerViewModel Grundgerüst | Alexei |
| 02.06.2026 | AddClientDialog, EditClientDialog, AddCardDialog | Alexei |
| 03.06.2026 | **Zwischenpräsentation** | Alexei & Chiara |
| 04.06.2026 | 4-Panel-System mit ManagerActiveView-Enum | Alexei |
| 05.06.2026 | UnauthorizedHandler, automatischer 401-Logout | Alexei |
| 09.06.2026 | Trailing-Slash-Bug behoben | Alexei |
| 10.06.2026 | MyInterestsView, ManagerInterestsView | Alexei |
| 11.06.2026 | StatsService, Statistik-Panel | Alexei |
| 12.06.2026 | LiveCharts2 Diagramme | Alexei |
| 13.06.2026 | AuditLog-Panel, EditProfileDialog | Alexei |
| 16.06.2026 | Serilog-Logging, xUnit-Tests, Manager-Rollenwahl, Kartenaktualisierung | Alexei |
| 17.06.2026 | **Endpräsentation / Abgabe** | Alexei & Chiara |
