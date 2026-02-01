# PetWorld - Sklep z produktami dla zwierząt

Aplikacja Blazor Server z asystentem AI opartym na architekturze Writer-Critic.

## Architektura

Projekt wykorzystuje Onion/Clean Architecture:

```
PetWorld/
├── src/
│   ├── PetWorld.Domain/           # Warstwa domenowa (encje, interfejsy)
│   ├── PetWorld.Application/      # Warstwa aplikacji (DTOs, serwisy)
│   ├── PetWorld.Infrastructure/   # Warstwa infrastruktury (EF Core, MySQL)
│   ├── PetWorld.AI/               # Warstwa AI (Writer-Critic agents)
│   └── PetWorld.Web/              # Warstwa prezentacji (Blazor Server)
├── docker-compose.yml
└── README.md
```

### Przepływ zależności

```
Web → Application → Domain ← Infrastructure
         ↓              ↑
        AI ─────────────┘
```

## Wymagania

- Docker i Docker Compose
- Klucz API OpenAI

## Uruchomienie

1. Ustaw zmienną środowiskową z kluczem OpenAI:

```bash
# Linux/macOS
export OPENAI_API_KEY=your-api-key-here

# Windows PowerShell
$env:OPENAI_API_KEY="your-api-key-here"

# Windows CMD
set OPENAI_API_KEY=your-api-key-here
```

2. Uruchom aplikację:

```bash
docker compose up --build
```

3. Otwórz przeglądarkę pod adresem: http://localhost:5000

## Funkcjonalności

### Chat (`/`)
- Pole tekstowe do zadawania pytań o produkty dla zwierząt
- Asystent AI rekomenduje produkty z katalogu PetWorld
- Wyświetlanie liczby iteracji Writer-Critic

### Historia (`/history`)
- Tabela z historią wszystkich rozmów
- Kolumny: Data, Pytanie, Odpowiedź, Iteracje

## Architektura AI (Writer-Critic)

Aplikacja wykorzystuje wzorzec Writer-Critic do generowania odpowiedzi:

1. **Writer Agent** - generuje odpowiedź na pytanie klienta
2. **Critic Agent** - ocenia odpowiedź pod kątem:
   - Poprawności informacji
   - Profesjonalnego tonu
   - Kompletności odpowiedzi
3. Jeśli Critic nie zaakceptuje odpowiedzi, Writer poprawia ją na podstawie feedbacku
4. Maksymalnie 3 iteracje

## Technologie

- .NET 7.0
- Blazor Server
- Entity Framework Core
- MySQL 8.0 (Pomelo.EntityFrameworkCore.MySql)
- OpenAI API (GPT-4o)
- Docker

## Struktura bazy danych

### Tabela `ChatHistories`
| Kolumna | Typ | Opis |
|---------|-----|------|
| Id | int | Klucz główny |
| Question | varchar(2000) | Pytanie użytkownika |
| Answer | TEXT | Odpowiedź AI |
| IterationCount | int | Liczba iteracji Writer-Critic |
| CreatedAt | datetime | Data utworzenia |

## Konfiguracja

Konfiguracja w `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=petworld;User=root;Password=root;"
  },
  "OpenAI": {
    "ApiKey": "your-api-key-here"
  }
}
```

W środowisku Docker konfiguracja jest przekazywana przez zmienne środowiskowe.

## Rozwój lokalny

1. Uruchom MySQL (np. przez Docker):
```bash
docker run -d --name mysql -e MYSQL_ROOT_PASSWORD=root -e MYSQL_DATABASE=petworld -p 3306:3306 mysql:8.0
```

2. Ustaw klucz OpenAI w `appsettings.json` lub zmiennej środowiskowej

3. Uruchom aplikację:
```bash
cd src/PetWorld.Web
dotnet run
```

## Licencja

MIT
