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
- **Production**: Klucz API OpenAI
- **Development**: Ollama (wbudowana w docker-compose)

## Uruchomienie

### Tryb DEVELOPMENT (Ollama - bez klucza API)

Idealny do lokalnego testowania. Uzywa Ollama z modelem llama3.2.

```bash
# Uruchom z profilem dev (wlacza Ollama + web-dev)
docker-compose --profile dev up -d --build

# Lub przebuduj tylko web-dev
docker-compose --profile dev up -d --build web-dev
```

Dostep: http://localhost:5001

> **Uwaga**: Pierwsze uruchomienie moze trwac dluzej - Ollama pobiera model (~2GB).

> **Bonus**: Open WebUI dostepne pod http://localhost:3100 - interfejs do zarzadzania Ollama.

### Tryb PRODUCTION (OpenAI)

Wymaga klucza API OpenAI. Uzywa modelu GPT-4o.

```bash
# 1. Ustaw klucz API
# Linux/macOS
export OPENAI_API_KEY=sk-your-api-key-here

# Windows PowerShell
$env:OPENAI_API_KEY="sk-your-api-key-here"

# Windows CMD
set OPENAI_API_KEY=sk-your-api-key-here

# 2. Uruchom aplikacje
docker-compose up -d --build web
```

Dostep: http://localhost:5000

### Porownanie trybow

| Cecha | Development | Production |
|-------|-------------|------------|
| Port | 5001 | 5000 |
| AI Provider | Ollama (llama3.2) | OpenAI (GPT-4o) |
| Wymaga klucza API | Nie | Tak |
| Jakosc odpowiedzi | Dobra (lokalne LLM) | Najlepsza |
| Koszty | Bezplatne | Platne (OpenAI) |
| Komenda | `--profile dev` | domyslnie |

### Przydatne komendy Docker

```bash
# Sprawdz logi aplikacji
docker-compose logs -f web        # Production
docker-compose logs -f web-dev    # Development

# Zatrzymaj wszystko
docker-compose --profile dev down

# Przebuduj po zmianach w kodzie
docker-compose up -d --build web
docker-compose --profile dev up -d --build web-dev

# Sprawdz status kontenerow
docker ps
```

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
- OpenAI API (GPT-4o) - Production
- Ollama (llama3.2) - Development
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

### appsettings.json (Production)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=petworld;User=root;Password=root;"
  },
  "OpenAI": {
    "ApiKey": "sk-your-api-key-here"
  }
}
```

### appsettings.Development.json (Development)

```json
{
  "Ollama": {
    "Endpoint": "http://localhost:11434/v1/",
    "Model": "llama3.2:latest"
  }
}
```

### Zmienne srodowiskowe

| Zmienna | Opis | Przyklad |
|---------|------|----------|
| `ASPNETCORE_ENVIRONMENT` | Tryb aplikacji | `Development` lub `Production` |
| `OpenAI__ApiKey` | Klucz API OpenAI | `sk-...` |
| `Ollama__Endpoint` | URL API Ollama | `http://localhost:11434/v1/` |
| `Ollama__Model` | Model Ollama | `llama3.2:latest` |
| `ConnectionStrings__DefaultConnection` | Connection string MySQL | `Server=...` |

## Rozwoj lokalny (bez Docker)

### Opcja 1: Development z Ollama

```bash
# 1. Zainstaluj i uruchom Ollama (https://ollama.ai)
ollama serve

# 2. Pobierz model
ollama pull llama3.2:latest

# 3. Uruchom MySQL
docker run -d --name mysql -e MYSQL_ROOT_PASSWORD=root -e MYSQL_DATABASE=petworld -p 3306:3306 mysql:8.0

# 4. Uruchom aplikacje w trybie Development
cd src/PetWorld.Web
set ASPNETCORE_ENVIRONMENT=Development
dotnet run
```

### Opcja 2: Production z OpenAI

```bash
# 1. Uruchom MySQL
docker run -d --name mysql -e MYSQL_ROOT_PASSWORD=root -e MYSQL_DATABASE=petworld -p 3306:3306 mysql:8.0

# 2. Ustaw klucz OpenAI
set ASPNETCORE_ENVIRONMENT=Production
set OpenAI__ApiKey=sk-your-api-key-here

# 3. Uruchom aplikacje
cd src/PetWorld.Web
dotnet run
```

## Licencja

MIT
