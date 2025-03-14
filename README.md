# DMR2Mongo

DMR2Mongo is a simple service that checks for updates to the Danish Vehicle Registry (DMR) database, downloads the latest available data, parses the objects, and stores them in a MongoDB database. The service utilizes [DMR.NET](https://github.com/j4asper/DMR.NET) to handle DMR data retrieval and parsing.