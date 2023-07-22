# DIRS21
# Introduction 
DIRS21 is a restful API for an availability system for hotel management.
This project implements a micro-service architecture, internally tightly cohesive DIRS21d externally loosely coupled.

# Getting Started
1. Editor
Use either VS Code or Visual Studio 2019(latest release) to contribute.

2.	Dependencies
 1. Target Framework - .NET 5.0
 2. Library Fx - .NetStDIRS21dard 2.1
 3. Runtime - .NET Core 5.0.1
 4. SDK - .NET 5 Version 5.0.101
 5. Latest Nuget Packages 


# Solution Architecture
The solution uses Clean Code architecture.
https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html


# Core.Domain(Contain database configuration classes)
  1. Entity
	- Contain definitions for schema classes that will be used in creating collections in our document database
  2. Enums
	- Central location for defining constants used as properties in entities
  3. Exception
	- Defining a global class that will be instantiated to throw handled exceptions
  4. Infrastructure
	1. Services
	  - Define DateTimeService used across the application for date purposes
  6. Dependency injection 
	- Contain an extension method  used for dependency injection of mongodb client to initiate connection to the MongoDB

# Core.Management(Contain logic for implementing business requirements)
  1. Common
	- Contain common classes reused across repositories are defined
  2. Extensions
	- Contain object extension methods used across the repositories
  3. Infrastructure
	- Contain intergration event publishers and handlers are defined
  4. Interfaces
	- Contains contract definition of repositories interfaces
  5. Repository
	- Contains implementation of generic repos used across the business logic
  6. DependencyInjection
	- Contain an extension method  used for dependency injection of repositories, event publishers and subscribers
  
# Services(Contain clients to our business logic either web api, background jobs, Webapps)
  # DIRS21.API
   1. Properties
	    - Contain the configuration information, which describes how to start the ASP.NET Core application, using Visual Studio
   2. Controllers
	    - Contain controllers used to define endpoints that will be accessed externally over the web
   3. Attributes
      - Contain attributes that are used to decorate controllers and views for either documentation or accessibility
   4. Common
	    - Contain definition of common objects reused in api
   5. Filters
		- Used to define custom made filters used to allow code to run before or after specific stages in the request processing pipeline.
		- An exception filter that runs when exceptions are thrown
		- An modelstate filter that runs when model request submitted are not valid
		- Swagger based filters for swagger documentation
   6. Models
   7. DTOS
	- Contain definition for request and return objects used in our action
	- Contain an automapper class used to map our entities to Dtos
   8. Program
	- Contain logging configurations and invocation of seed methods
   9. Start
	 - Calls the dependency injection extension methods in the Common.Domain and Common.Management project
	 - Contain API versioning dependency injection logic
	 - Contain filter dependency injection logic
	 - Contain Swagger documentation logic
	 - Contain JWT Security Pipeline logic and its dependency injection

 # Unit tests
	- Contain unit test logic for our controllers and repositories
	- Uses X unit test tool
	- Uses FluentAssertion nugets for assertions
	- Uses FakeItEasy nugets for mocking dependencies and objects
