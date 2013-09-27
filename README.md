GenericData
===========

Reflection based C# data access experiments. DataUtils is the most interesting C# project there.

Concept
===========

In theory there is a 'DataObject,' which has nothing but an ID.
There is also a GenericDAO that can work with any variant of a DataObject.

That GenericDAO may be a SqlDAO, XmlDAO, or AccessDAO - the rest of the code doesn't care. Release code may use a SqlDAO while test code will use an XmlDAO or AccessDAO during testing to confirm against a specific datasources.

The objective is true multi-tier (which I personally define as hot-swappable datasources) and not just the hardcoded 3-tier (datasource, datamodel, and view) that manages to pass as 'Enterprise' or 'Multi-tier.' 

In practice the model and the view cannot realistically be seperated without introducing 'developer hell' or resorting to the language's reflection/annotation mechanisms to determine the view.

Crazy things it does
===========

The Github version doesn't do it, but my own version queries the db table (doesn't care about xml) to remove or delete columns based on their prescense in the class.

Cool things I'm doing that you could do
===========

For objects that are either too complex or not search worthy - I just dump them into a JSON string and write that to the database in a varchar(MAX) field. JSON is pretty compact in comparison to XML. 

I'm taking this approach in mobile development, I have tables where there's only an integer ID and a max string field containing JSON.
