# API specification

| GetDrivers | /drivers |
| ----------- | ----------- |
| Response codes | 200 - Ok |
| Method | GET |
| Parameters |  |
| Response | Json |

### Example request:
```
GET /drivers
```
### Example response:
```
[
    {
        "id": 10,
        "name": "Jonas Jonaitis",
        "email": "jonas.jonaitis@example.com",
        "phoneNumber": "+37060000011"
    },
    {
        "id": 11,
        "name": "Marius Marijauskas",
        "email": "marius.marijauskas@example.com",
        "phoneNumber": "+37060000012"
    },
    {
        "id": 12,
        "name": "Elena Petrikaitė",
        "email": "elena.petrikaite@example.com",
        "phoneNumber": "+37060000013"
    }
]
```

---
| GetDriver | /drivers/{driverId} |
| ----------- | ----------- |
| Response codes | 200 - Ok, 404 - Not Found |
| Method | GET |
| Parameters | driverId |
| Response | Json |

### Example request:
```
GET /drivers/10
```
### Example response:
```
{
    "id": 10,
    "name": "Jonas Jonaitis",
    "email": "jonas.jonaitis@example.com",
    "phoneNumber": "+37060000011"
}
```

---
| CreateDriver | /drivers |
| ----------- | ----------- |
| Response codes | 201 - Created |
| Method | POST |
| Parameters |  |
| Response | Json |

### Example request:
```
POST /drivers

Body:
{
    "name": "Kęstutis Kęstutaitis",
    "email": "kestutis.kestutaitis@example.com",
    "password": "password456",
    "phoneNumber": "+37060000020"
}
```
### Example response:
```
{
    "id": 20,
    "name": "Kęstutis Kęstutaitis",
    "email": "kestutis.kestutaitis@example.com",
    "phoneNumber": "+37060000020"
}
```

---
| UpdateDriver | /drivers/{driverId} |
| ----------- | ----------- |
| Response codes | 200 - Ok, 404 - Not Found |
| Method | PUT |
| Parameters | driverId |
| Response | Json |

### Example request:
```
PUT /drivers/20

Body:
{
    "email": "newemail@example.com",
    "password": "password456",
    "phoneNumber": "+37060000020"
}
```
### Example response:
```
{
    "id": 20,
    "name": "Kęstutis Kęstutaitis",
    "email": "newemail@example.com",
    "phoneNumber": "+37060000020"
}
```
---
| DeleteDriver | /drivers/{driverId} |
| ----------- | ----------- |
| Response codes | 204 - No Content, 404 - Not Found |
| Method | DELETE |
| Parameters | driverId |
| Response | Json |

### Example request:
```
DELETE /drivers/20
```
---
| GetTrips | /drivers/{driverId}/trips |
| ----------- | ----------- |
| Response codes | 200 - Ok |
| Method | GET |
| Parameters | driverId |
| Response | Json |

### Example request:
```
GET /drivers/10/trips
```
### Example response:
```
[
    {
        "id": 10,
        "departure": "Šiauliai",
        "destination": "Vilnius",
        "time": "2023-10-17T11:30:00",
        "seats": 3,
        "description": "Business meeting"
    },
    {
        "id": 11,
        "departure": "Kaunas",
        "destination": "Palanga",
        "time": "2023-10-18T15:00:00",
        "seats": 2,
        "description": "Beach day"
    },
    {
        "id": 12,
        "departure": "Panevėžys",
        "destination": "Klaipėda",
        "time": "2023-10-19T13:20:00",
        "seats": 4,
        "description": "Scenic route"
    }
]
```

---
| GetTrip | /drivers/{driverId}/trips/{tripId} |
| ----------- | ----------- |
| Response codes | 200 - Ok, 404 - Not Found |
| Method | GET |
| Parameters | driverId, tripId |
| Response | Json |

### Example request:
```
GET /drivers/10/trips/10
```
### Example response:
```
{
    "id": 10,
    "departure": "Šiauliai",
    "destination": "Vilnius",
    "time": "2023-10-17T11:30:00",
    "seats": 3,
    "description": "Business meeting"
}
```

---
| CreateTrip | /drivers/{driverId}/trips |
| ----------- | ----------- |
| Response codes | 201 - Created |
| Method | POST |
| Parameters | driverId |
| Response | Json |

### Example request:
```
POST /drivers/10/trips

Body:
{
    "departure": "Vilnius",
    "destination": "Kaunas",
    "time": "2023-10-21T10:30:00",
    "seats": 2,
    "description": "City tour"
}
```
### Example response:
```
{
    "id": 15,
    "departure": "Vilnius",
    "destination": "Kaunas",
    "time": "2023-10-21T10:30:00",
    "seats": 2,
    "description": "City tour"
}
```

---
| UpdateTrip | /drivers/{driverId}/trips/{tripId} |
| ----------- | ----------- |
| Response codes | 200 - Ok, 404 - Not Found |
| Method | PUT |
| Parameters | driverId, tripId |
| Response | Json |

### Example request:
```
PUT /drivers/10/trips/15

Body:
{
    "time": "2023-10-21T10:30:00",
    "seats": 2,
    "description": "New description"
}
```
### Example response:
```
{
    "id": 15,
    "departure": "Vilnius",
    "destination": "Kaunas",
    "time": "2023-10-21T10:30:00",
    "seats": 2,
    "description": "New description"
}
```
---
| DeleteTrip | /drivers/{driverId}/trips/{tripId} |
| ----------- | ----------- |
| Response codes | 204 - No Content, 404 - Not Found |
| Method | DELETE |
| Parameters | driverId, tripId |
| Response | Json |

### Example request:
```
DELETE /drivers/10/trips/15
```

---
| GetReviews | /drivers/{driverId}/trips/{tripId}/reviews |
| ----------- | ----------- |
| Response codes | 200 - Ok |
| Method | GET |
| Parameters | driverId, tripId |
| Response | Json |

### Example request:
```
GET /drivers/10/trips/12/reviews
```
### Example response:
```
[
    {
        "id": 13,
        "rating": 4,
        "description": "Clean and well-maintained vehicle.",
        "reviewerId": 16
    },
    {
        "id": 14,
        "rating": 1,
        "description": "Terrible experience, never again.",
        "reviewerId": 17
    },
    {
        "id": 15,
        "rating": 4,
        "description": "Efficient route taken by the driver.",
        "reviewerId": 18
    },
    {
        "id": 16,
        "rating": 3,
        "description": null,
        "reviewerId": 19
    }
]
```

---
| GetReview | /drivers/{driverId}/trips/{tripId}/reviews/{reviewId} |
| ----------- | ----------- |
| Response codes | 200 - Ok, 404 - Not Found |
| Method | GET |
| Parameters | driverId, tripId, reviewId |
| Response | Json |

### Example request:
```
GET /drivers/10/trips/12/reviews/13
```
### Example response:
```
{
    "id": 13,
    "rating": 4,
    "description": "Clean and well-maintained vehicle.",
    "reviewerId": 16
}
```

---
| CreateReview | /drivers/{driverId}/trips/{tripId}/reviews |
| ----------- | ----------- |
| Response codes | 201 - Created |
| Method | POST |
| Parameters | driverId, tripId |
| Response | Json |

### Example request:
```
POST /drivers/10/trips/12/reviews

Body:
{
    "rating": 3,
    "description": null,
    "reviewerid": 19
}
```
### Example response:
```
{
    "id": 17,
    "rating": 3,
    "description": null,
    "reviewerId": 19
}
```

---
| UpdateTrip | /drivers/{driverId}/trips/{tripId}/reviews/{reviewId} |
| ----------- | ----------- |
| Response codes | 200 - Ok, 404 - Not Found |
| Method | PUT |
| Parameters | driverId, tripId, reviewId |
| Response | Json |

### Example request:
```
PUT /drivers/10/trips/12/reviews/17

Body:
{
    "rating": 3,
    "description": "new description"
}
```
### Example response:
```
{
    "id": 17,
    "rating": 3,
    "description": "new description",
    "reviewerId": 19
}
```
---
| DeleteReview | /drivers/{driverId}/trips/{tripId}/reviews/{reviewId} |
| ----------- | ----------- |
| Response codes | 204 - No Content, 404 - Not Found |
| Method | DELETE |
| Parameters | driverId, tripId, reviewId |
| Response | Json |

### Example request:
```
DELETE /drivers/10/trips/12/reviews/17
```
