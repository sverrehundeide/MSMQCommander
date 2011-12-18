TODO
====

Next release
------------
* Automatic refresh of messages
* Support for transactional queues

Future release
--------------
* Show message extension data
* Queue ping (send message with a correlation id and receive it back with ReceiveByCorrelationId)
* Queue path builder for direct format name, private queues (enter machine name, protocol, http/https)
* Text search in message labels
* Text search in message body
* Filter messages based on label text
* Filter messages based on body contents
* Change message contents and resubmit message to queue (text serialization only)
* Support for binary serialized messages