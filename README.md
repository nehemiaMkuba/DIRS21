# DIRS21

# Solution Architecture
The solution uses Clean Code architecture.
https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html


# Core.Domain(Contain database configuration classes)
 # Entity
   - Contain definitions for schema classes that will be used in creating collections in our document database
 # Enums
   - Central location for defining constants used as properties in entities
 # Exception
   - Defining a global class that will be instantiated to throw handled exceptions
 # Infrastructure
	# Services
	  - Define DateTimeService used across the application for date purposes
 # Dependency injection 
   - Contain an extension method  used for dependency injection of mongodb client to initiate connection to the MongoDB

# Core.Management(Contain logic for implementing business requirements)
  # Common
   - Contain common classes reused across repositories are defined
  # Extensions
   - Contain object extension methods used across the repositories
  # Implementations
   - Contain concrete implentantions of repositories
  # Infrastructure
   - Contains intergration event publishers and handlers are defined
  # Interfaces
   - Contains contract definition of repositories interfaces
  # Repository
   -Contains implementation of generic repos used across the business logic
  # DependencyInjection
   -Contain an extension method  used for dependency injection of repositories, event publishers and subscribers
  
# Services(Contain clients to our business logic either web api, background jobs, Webapps)
  # DIRS21.API
   # Properties
	    - Contain the configuration information, which describes how to start the ASP.NET Core application, using Visual Studio
   # Controllers
	    - Contain controllers used to define endpoints that will be accessed externally over the web
   # Attributes
      - Contain attributes that are used to decorate controllers and views for either documentation or accessibility
   # Common
	    - Contain definition of common objects reused in api
   # Filters
	   - Used to define custom made filters used to allow code to run before or after specific stages in the request processing pipeline.
	   - An exception filter that runs when exceptions are thrown
	   - An modelstate filter that runs when model request submitted are not valid
	   - Swagger based filters for swagger documentation
   # Models
   # DTOS
	  - Contain definition for request and return objects used in our action
	  - Contain an automapper class used to map our entities to Dtos
  # Program
     - Contain logging configurations and invocation of seed methods
  # Start
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
