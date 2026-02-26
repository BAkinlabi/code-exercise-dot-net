# URL Shortener API

A simple URL shortener RESTful API built with .NET 8, following SOLID principles and best practices.

## How to Build and Run Locally

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (optional, for containerized run)

### Running with .NET CLI

### 1. Clone the repository and navigate to the API project folder:

cd UrlShortenerApi

### 2. Restore dependencies:

dotnet restore

### 3. Build the project:

dotnet build

### 4. Run the API:

dotnet run

The API will start on `http://localhost:8080` by default.

### Running with Docker

### 1. Build the Docker image:

docker build -t urlshortenerapi

### 2. Run the container:

docker run -p 8080:8080 urlshortenerapi

## Example API Usage

### Shorten a URL

**Request:**

curl -X POST http://localhost:8080/shorten 
-H "Content-Type: application/json" 
-d '{"fullUrl": "https://mytestwebsite.com/very/long/url"}'

**Response:** 

{ "shortUrl": "http://localhost:8080/abc123" }

### Shorten with Custom Alias

**Request:**

curl -X POST http://localhost:8080/shorten 
-H "Content-Type: application/json" 
-d '{"fullUrl": "https://mytestwebsite.com/very/long/url", "customAlias": "my-custom-alias"}'

**Response:** 

{ "shortUrl": "http://localhost:8080/my-custom-alias" }

### Redirect to Full URL

**Request:**

curl -v http://localhost:8080/my-custom-alias

**Response:** 

HTTP 302 redirect to the original URL.

### List All Shortened URLs

**Request:**

curl http://localhost:8080/urls

**Response:**

[ { "alias": "my-custom-alias", "fullUrl": "https://mytestwebsite.com/very/long/url", "shortUrl": "http://localhost:8080/my-custom-alias" } ]

### Delete a Shortened URL

**Request:**

curl -X DELETE http://localhost:8080/my-custom-alias

**Response:**  
HTTP 204 No Content

## Notes & Assumptions

- **Persistence:** URLs are persisted to a local file (`json-url-datastore.json`) in the API directory. This is suitable for demo and development purposes.
- **Validation:** The API validates that `fullUrl` is a valid absolute URL and that custom aliases are unique.
- **Error Handling:** Returns HTTP 400 for invalid input or duplicate aliases, 404 for not found, and 302 for redirects.
- **Port:** The API listens on port `8080` by default.
- **Logging:** Serilog is configured for logging; logs are written to the `Logs` directory.
- **OpenAPI/Swagger:** Swagger UI is available at `/swagger` when running locally for API exploration.
- **Testing:** Unit tests should be added in a separate test project (not included in this snippet).
- **No authentication** is implemented; all endpoints are public.
- **Containerization:** A Dockerfile is provided for easy containerized deployment.

---
    