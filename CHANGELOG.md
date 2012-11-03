Changelog
=========

For next release
----------------


2012-11-03: Version 0.5 (Alpha release)
---------------------------------------
* Support for creating new queues
* Support for deleting queues
* FIX: Handle situations where the user does not have access to a queue (queue name was set to empty value and exception was thrown)

2012-04-01: Version 0.4 (Alpha release)
---------------------------------------
* Support for transactional queues
* Use Delete key in grid view to delete one or more selected message(s)
* Added accelerator keys for buttons and set default focused text box in dialogs
* Made gridview panel resizable
* Added button for auto refresh of messages in open queues and of message count and queue names in tree view
* Switched to WIX installer (old installer no longer works in Visual Studio 11)
* Changed installer to support upgrade of previously installed version

2011-12-18: Version 0.3 (Alpha release)
---------------------------------------
* Insert new message to queue (body serialized as text/xml only)
* Copying message to file
* Importing message from file
* Connect to remote queues
* Context menu option for delete messages in grid view

2011-10-09: Version 0.2 (Alpha release)
---------------------------------------
* View journal queue as child nodes in queue tree view
* View Number of messages in queueu behind queue name in tree view
* Switch journaling on/off
* Purge messages in queue

2011-09-25: Version 0.1 (Alpha release)
---------------------------------------
* View private queues on local computer
* View messages in queues
* View properties for a selected message
* View message body (only text deserialization is supported for now)