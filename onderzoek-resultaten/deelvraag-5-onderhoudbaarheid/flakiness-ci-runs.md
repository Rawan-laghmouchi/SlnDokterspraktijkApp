# Flakinessmeting via GitHub Actions

Voor de stabiliteitsmeting werden de xUnit-tests en Reqnroll-tests meerdere keren uitgevoerd via GitHub Actions. De workflow voerde beide testsets tien keer uit.

Warnings en annotations werden niet als falende runs beschouwd, omdat de testuitvoering zelf succesvol werd afgerond.

## Meetresultaten flakiness

| Branch | Testbenadering | Geslaagde runs | Gefaalde runs | Flakiness |
|--------|----------------|----------------|---------------|-----------|
| main   | xUnit          | 10/10          | 0/10          | 0%        |
| main   | Reqnroll       | 10/10          | 0/10          | 0%        |

## Interpretatie

Binnen deze meting werd geen wisselend testgedrag vastgesteld. Zowel de xUnit-tests als de Reqnroll-tests slaagden tien keer op tien. 
De gemeten flakiness bedroeg daardoor voor beide testbenaderingen 0%.