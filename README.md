# Table of Contents

- [About the Project](#about-the-project)
  - [Tech Stack](#tech-stack)
  - [Features](#features)
- [Roadmap](#roadmap)
- [Contact](#contact)

<!-- About the Project -->

## About the Project

<!-- TechStack -->

### Tech Stack

<details>
  <summary>API / Server</summary>
  <ul>
    <li><a href="https://learn.microsoft.com/en-us/dotnet/csharp/">C#</a></li>
    <li><a href="https://dotnet.microsoft.com/en-us/apps/aspnet">ASP.NET</a></li>
    <li><a href="https://docs.fluentvalidation.net/en/latest/">FluentValidation</a></li>
    <li><a href="https://github.com/mcintyre321/OneOf">OneOf</a></li>
    <li><a href="https://supabase.com/docs/reference/csharp/introduction">C# Supabase Client</a></li>
  </ul>
</details>

<details>
<summary>Database</summary>
  <ul>
    <li><a href="https://supabase.com">Supabase</a></li>
  </ul>
</details>

<details>
<summary>DevOps</summary>
  <ul>
    <li><a href="https://www.docker.com/">Docker</a></li>
  </ul>
</details>

<!-- Features -->

### Features

- **Searchable Cocktail Database**  
  Fetch curated cocktail data with detailed ingredients and instructions.

- **Modular Ingredient Management**  
  Ingredients are stored independently for easy recipe composition and reuse.

- **Ratings System**
  Authenticated users can leave ratings on cocktails and update/delete their existing ratings.

- **User Authentication**  
  Token-based authentication for user access and personalization.

- **Clean Architecture**  
  Separation of concerns with a dedicated application layer, mappers, and validation pipeline.

<!-- Roadmap -->

## Roadmap

- [ ] Add favorites functionality
- [ ] Add comments functionality
- [ ] Add cocktail builder to see what cocktails can be made with different ingredients
  - [ ] Users will have their own inventory that will show them what cocktails they are able to make with the ingredients in their bar
- [ ] Allow users to create their own variants of existing cocktails
  - [ ] User will have choice to view their variant first instead of the original recipe
- [ ] Add cocktail creator functionality to allow authenticated users to create their own recipes
  - [ ] Can be made public or private for others to view
- [ ] Add redis output cache to reduce DB calls

<!-- Contact -->

## Contact

Bailey Frye - baileyafrye@comcast.net
