## Auction filters
This projects contains filters used for filtering auctions on https://sky.coflnet.com

Use cases:
* Filtering price history from database
* Checking if auction matches for white/blacklisting
    * notifications
    * flipper
    * sniper  

Filters are string based and consist of a `key` and `value`.
Filters have options that tell you what values are valid for `value`.
There are varios types of filters such as `range` `match` and numberic.  
These types can also be combined.

## Deploying
This project should be deployed within a container. 
### Configuration
There are currently no configuration options.
