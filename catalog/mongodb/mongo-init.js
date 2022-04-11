db.createUser({
    user: 'catalogapp',
    pwd: 'gift932',
    roles: [
      { role: 'dbOwner', db: 'catalog'},
      { role: 'readWrite', db: 'catalog'}
    ]});

db.createCollection('items');