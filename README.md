# Finbourne Core
## Components
### LRU Cache

The LRU implementation is based on the following Java example
https://www.baeldung.com/java-lru-cache

    How can we design a data structure that could do operations like reading, sorting (temporal sorting), and deleting elements in constant time?

    It seems that to find the answer to this question, we need to think deeply about what has been said about LRU cache and its features:
     - In practice, LRU cache is a kind of Queue — if an element is reaccessed, it goes to the end of the eviction order
     - This queue will have a specific capacity as the cache has a limited size. Whenever a new element is brought in, it is added at the head of the queue. When eviction happens, it happens from the tail of the queue.
     - Hitting data in the cache must be done in constant time, which isn't possible in Queue! But, it is possible with Java's HashMap data structure
     - Removal of the least recently used element must be done in constant time, which means for the implementation of Queue, we'll use DoublyLinkedList instead of SingleLinkedList or an array
    
    So, the LRU cache is nothing but a combination of the DoublyLinkedList and the Dictionary