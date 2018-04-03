# UXR

###### The web application of the UXI Group Studies system.

UXR is designed for administration of group studies recorded with the [UXC client application](https://github.com/uxifiit/UXC).
UXC client and UXR web management applications form the UXI Group Studies infrastructure for conducting group eye tracking studies. 
This project is developed at [User eXperience and Interaction Research Center](https://www.uxi.sk/) of [Slovak University of Technology in Bratislava](http://fiit.stuba.sk/)

UXR is an ASP.NET MVC web application with two major components (areas):
* *Studies* - administration of group studies - manage projects, sessions, recordings. 
* *Packages* - distribution of the UXC application updates (TBA).

## Main features

* Set up a new study project - create Project with an UXC session recording definition.
* Schedule sessions of the experiment, modify session definitions individually for each session if needed.
* Distribute session definition to UXC nodes.
* Remotely observe UXC nodes in the classroom on a dashboard.
* Control the UXC nodes in the classroom.
* Download recorded data after the recording.

<p><img src="docs/recording-remote.png" alt="Overview of remotely controlled session recording on UXC nodes from UXR" height="300" /></p>

## Solution structure

Source code folder `src` contains the following projects:

* UXR - the main app project.
* UXR.Common - helper classes.
* UXR.Models & UXR.Models.Entities - the main app EF Code First model including models of all the areas used in the app. 
* UXR.Studies - class library with `Studies` area for the main app, contains web Controllers for administration of group studies. 
* UXR.Studies.Api - WebApi controllers for `Studies` area.
* UXR.Studies.Api.Client - API client library, exposed as a NuGet package for other projects.
* UXR.Studies.Models - models for `Studies` area. 

When adding new functionality, consider if it belongs to the main app, `Studies` area, or a new area. 

## Contributing

Use [Issues](issues) to request features, report bugs, or discuss ideas.

## Dependencies

* ASP.NET MVC 5, WebApi, Owin
* [UXI.Libs](https://github.com/uxifiit/UXI.Libs)
* [Json.NET](https://github.com/JamesNK/Newtonsoft.Json)
* [Ninject](https://github.com/ninject/Ninject)
* [AutoMapper](https://github.com/AutoMapper/AutoMapper)
* Node.js
* and others through NPM 

## How to build


### Configure

Create these configuration files in the main UXR app project, based on the examples in `*.sample` files:
* `connectionStrings.config` - connection string for the database in the MSSQL Server.
* `deployment.config` - specify path where to store recordings and first user's (super admin) name and password.

### Bower bundles

For the UXR web app, NuGet was replaced with Bower for resolving JavaScript packages. See Developer notes on the Wiki pages for more information. 

These files were added to the default ASP.NET MVC 5 project template:
* **bower.json** - contains dependencies on JavaScript libraries.
* **.bowerrc** - contains location where to put downloaded libraries, set to **bower_components**.
* **gruntfile.js** - specifies grunt tasks.
* **package.json** - node.js configuration file.
* **App_Data/BowerBundles.json** - configuration file for Bower bundles._

Package restore is required before building, since the **bower_components** directory is set to be ignored by Git. 

To use Bower bundles with scripts and styles in Razor views, use:
* **@Html.HtmlCssCached("bundleName")** - equivalent to @Styles.Render("~/Content/bundleName")
* **@Html.HtmlScriptsCached("bundleName")** - equivalent to @Scripts.Render("~/bundles/bundleName")

### Restore and build Bower bundles

Bower bundles must be restored and built before running the app, either from command line or Visual Studio:

* From command line, run these commands to prepare publish package with **css, js, fonts** folders:

```
$ npm install -g bower
$ npm install -g grunt-cli

$ npm install
$ bower install --force-latest
$ grunt build
```

* In Visual Studio:
    1. Right click on the **package.json** file and `Restore Packages`.
    2. Right click on the **bower.json** file and `Restore Packages`.
    2. Open the **Task Runner Explorer** window in Visual Studio.
    2. Run the **build** task (double-click or right click and Run).
    3. Build task generates **css, js, fonts** folders. 


## Authors

* Martin Konopka - [@martinkonopka](https://github.com/martinkonopka)
* Peter Demcak  

## License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details

## Contacts

* UXIsk 
  * User eXperience and Interaction Research Center, Faculty of Informatics and Information Technologies, Slovak University of Technology in Bratislava
  * Web: https://www.uxi.sk/
* Martin Konopka
  * E-mail: martin (underscore) konopka (at) stuba (dot) sk



