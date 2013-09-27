GenericData
===========

Reflection based C# data access experiments. DataUtils is the most interesting C# project there. If the object has changed the DAO will automatically (for Access and SQL) make the necessary updates to the table.

Concept
=========

In theory there is a 'DataObject,' which has nothing but an ID.
There is also a GenericDAO that can work with any variant of a DataObject. The 'Loader' utils class uses a DAO's "emulate" methods and some reflection to fully load or save a class tree - dealing with nested DataObjects that belong in seperate tables or lists (extendsing IList) of DataObjects in a one to many relationship.

One-to-many relationships are not resolved in a proper normal form. The ID's of the DataObject's in the IList are written into a string field, and 'blank' instances are generated on read based on those ids. The Loader class than uses those blanks to and their IDs to get the 'full' versions.

That GenericDAO may be a SqlDAO, XmlDAO, or AccessDAO - the rest of the code doesn't care. Release code may use a SqlDAO while test code will use an XmlDAO or AccessDAO during testing to confirm against a specific datasources.

The objective is generic data-access that could easily be hot-pluggable and uses reflection to bind to plain-old-data types.

Cool things I'm doing that you could do
=========

For objects that are either too complex or not search worthy or that need normalization - I just dump them into a JSON string and write that to the database in a varchar(MAX) field. JSON is pretty compact in comparison to XML. (they're identified by annotations/attributes)

I'm taking this approach in mobile development, as it's makes for considerably less work when writing code for a table schema change as most of the 'non-mission-critical' information is in a JSON field or two/three/whatever while information that you'd likely want to query on for searching purposes such as "CustomerName" "Address" etc stand alone.

What's Really Bad
=========

IList, is treated as always containing DataObject derived types. That's bad. IList handling should support all of the supported types.