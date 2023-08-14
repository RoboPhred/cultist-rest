# CSRestAPI

Rest API server for interfacing with Cultist Simulator

# API Docs

## Endpoints

### Get All Spheres at the Root

- **URL**: `/api/by-path/~/spheres`
- **Method**: `GET`
- **Response**:
  - **Code**: 200 OK
  - **Content**: Array of spheres in JSON format

### Get All Spheres in a Token

- **URL**: `/api/by-path/{...path}/spheres`
- **Method**: `GET`
- **URL Parameters**:
  - `path`: The full absolute path of the token.
- **Response**:
  - **Code**: 200 OK
  - **Content**: Array of spheres in JSON format
- **Exceptions**:
  - 404 Not Found: If the path is not found.
  - 400 Bad Request: If the path resolves to something other than a token.

### Get All Tokens in a Sphere

- **URL**: `/api/by-path/{...path}/tokens`
- **Method**: `GET`
- **URL Parameters**:
  - `path`: The full absolute path of the sphere.
- **Query Parameters** (optional):
  - `payloadType`: Filter tokens by payload type.
  - `entityId`: Filter tokens by entity ID.
- **Response**:
  - **Code**: 200 OK
  - **Content**: Array of tokens in JSON format
- **Exceptions**:
  - 404 Not Found: If the sphere is not found.
  - 400 Bad Request: If the path resolves to something other than a sphere.

### Create a Token in a Sphere

- **URL**: `/api/by-path/{...path}/tokens`
- **Method**: `POST`
- **URL Parameters**:
  - `path`: The fucine path of a sphere.
- **Body**: JSON object or array representing the token.

  - The body can be a single object, or an array of objects to create multiple tokens in one go.
  - Each object can define either an ElementStack or a Situation
  - **ElementStack**

    - **payloadType**: Must be "ElementStack" for an element stack.
    - **elementId** (type: `string`, required)

      - Description: ID of the element to create a stack of.

    - **quantity** (type: `int`, required)
      - Description: Quantity of the element to create a stack of.
    - **mutations** (type: `object`, optional)
      - Description: Mutations to apply to the element stack.
      - Default: `{}` (empty dictionary)
      - Example:
        ```json
        {
          "lantern": 2,
          "edge": 3
        }
        ```

  - **Situation**
    - **payloadType**: Must be "Situation" for a situation token.
    - **verbId** (type: `string`, required if `recipeId` is not present)
      - Description: The id of the verb to create a situation of.
    - **recipeId** (type: `string`, optional)
      - Description: The id of the recipe to start the verb with.
      - Do not specify this if you want to spawn the verb idle.

- **Response**:
  - **Code**: 201 Created
  - **Content**: JSON representation of created token(s).
- **Exceptions**:
  - 404 Not Found: If the sphere is not found.
  - 400 Bad Request: If the request body is invalid, or if the path resolves to something other than a sphere.

### Modify a Token

- **URL**: `/api/by-path/{...path}`
- **Method**: `PUT`
- **URL Parameters**:
  - `path`: The fucine path of a token.
- **Body**: JSON object representing the updates to the token.
- **Response**:
  - **Code**: 200 OK
  - **Content**: JSON representation of the updated token.
- **Exceptions**:
  - 404 Not Found: If the sphere or token is not found.
  - 400 Bad Request: If the request body is invalid, or if the path resolves to something other than a token.

### Delete a Token

- **URL**: `/api/by-path/{...path}`
- **Method**: `DELETE`
- **Response**:
  - **Code**: 200 OK
- **Exceptions**:
  - 404 Not Found: If the token is not found.
  - 400 Bad Request: If the item cannot be deleted.

### Delete All Tokens in a Sphere

- **URL**: `/api/by-path/{...path}/tokens`
- **Method**: `DELETE`
- **URL Parameters**:
  - `path`: The fucine path of a sphere.
- **Response**:
  - **Code**: 200 OK
- **Exceptions**:
  - 404 Not Found: If the sphere is not found.
  - 400 Bad Request: If the path resolves to something other than a sphere.

## Game Speed

### GET /speed

- **Description**: Gets the current game speed.
- **Parameters**: None
- **Response**:
  ```json
  {
    "speed": "Normal"
  }
  ```
- **Possible Responses**:
  - `200 OK`: Speed returned successfully.
  - `409 Conflict`: The game is not in a running state.

### POST /speed

- **Description**: Sets the game speed.
- **Body**:
  ```json
  {
    "gameSpeed": "Fast"
  }
  ```
- **Possible Values**: `Paused`, `Normal`, `Fast`, `VeryFast`, `VeryVeryFast`
- **Response**: `200 OK`

## Game Time Control

### POST /beat

- **Description**: Advances the game by a fixed beat time.
- **Body**:
  ```json
  {
    "seconds": 10
  }
  ```
- **Response**: `200 OK`

## Game Events

### GET /events

- **Description**: Retrieves the timings for the next in-game events.
- **Response**:
  ```json
  {
    "nextCardTime": 5.2,
    "nextVerbTime": 7.6
  }
  ```
- **Response**: `200 OK`

### POST /events/beat

- **Description**: Fast-forwards the game to the next specified in-game event.
- **Body**:
  ```json
  {
    "event": "Either"
  }
  ```
- **Possible Values**: `CardDecay`, `RecipeCompletion`, `Either`
- **Response**:
  ```json
  {
    "secondsElapsed": 5.3
  }
  ```
- **Response**: `200 OK`

## Token JSON Format

Tokens contain the following properties

### Properties

#### `id`

- **Type**: `string`
- **Description**: The token's ID.
- **Example**: `"!funds_2024"`
- **Read-Only**

#### `path`

- **Type**: `string`
- **Description**: The fucine path of this token.
- **Example**: `"~tabletop/!funds_2024"`
- **Read-Only**

#### `spherePath`

- **Type**: `string`
- **Description**: The path of the sphere this token is contained in.
- **Example**: `"~tabletop"`
- **Readable**
- **Writable**
- **Notes**: If the sphere rejects the token, 409 Conflict will be returned. This may cause some of the other properties in your request to not be set.

#### `payloadType`

- **Type**: `enum`
- **Available values**
- `Situation` (see [Situation JSON Format](#situation-json-format))
- `ElementStack` (see [ElementStack JSON Format](#elementstack-json-format))
- **Description**: The token's payload type
- **Example**: `"tabletop"`
- **Read-only**

## ElementStack JSON Format

In addition to handling Token JSON properties, ElementStack tokens can handle the following properties.

### Properties

#### `elementId`

- **Type**: `string`
- **Description**: The ID of the element.
- **Example**: `"element_123"`
- **Read-only**

#### `quantity`

- **Type**: `int`
- **Description**: The quantity of the token.
- **Example**: `10`

#### `lifetimeRemaining`

- **Type**: `float`
- **Description**: The time remaining in seconds.
- **Example**: `15.5`
- **Read-only**

#### `aspects`

- **Type**: `JObject`
- **Description**: The aspects of the element stack.
- **Example**: `{ "AspectA": 5, "AspectB": 10 }`
- **Read-only**

#### `mutations`

- **Type**: `JObject`
- **Description**: The mutations of the element stack.
- **Example**: `{ "lantern": 3, "winter": 4 }`
- **Readable**
- **Writable**

#### `shrouded`

- **Type**: `bool`
- **Description**: Indicates if the token is shrouded.
- **Example**: `true`
- **Readable**
- **Writable**

#### `label`

- **Type**: `string`
- **Description**: The label of the element stack.
- **Example**: `"Element Stack Label"`
- **Read-only**

#### `description`

- **Type**: `string`
- **Description**: The description of the element stack.
- **Example**: `"Description of the element stack."`
- **Read-only**

#### `icon`

- **Type**: `string`
- **Description**: The icon of the element stack.
- **Example**: `"icon_path/icon_name.png"`
- **Read-only**

#### `uniquenessGroup`

- **Type**: `string`
- **Description**: The uniqueness group of the element stack.
- **Example**: `"GroupA"`
- **Read-only**

#### `decays`

- **Type**: `bool`
- **Description**: Determines whether the element stack decays.
- **Example**: `true`
- **Read-only**

#### `metafictional`

- **Type**: `bool`
- **Description**: Determines whether the element stack is metafictional.
- **Example**: `false`
- **Read-only**

#### `unique`

- **Type**: `bool`
- **Description**: Determines whether the element stack is unique.
- **Example**: `true`
- **Read-only**

### Situation JSON Format

In addition to handling Token JSON properties, situation tokens can handle the following properties.

#### `timeRemaining`

- **Type**: `float`
- **Description**: The time remaining in the situation's current recipe.
- **Example**: `23.5`
- **Read-only**

#### `recipeId`

- **Type**: `string`
- **Description**: The recipe ID of the situation's fallback recipe.
- **Example**: `"recipe_1234"`
  - **Exceptions**:
    - `BadRequestException`: Raised when the provided recipe ID is not found.
    - `ConflictException`: Raised when the situation is not in the correct state to begin a recipe.
- **Readable**
- **Writable**

#### `currentRecipeId`

- **Type**: `string`
- **Description**: The recipe ID of the situation's current recipe.
- **Example**: `"recipe_5678"`
- **Read-only**

#### `state`

- **Type**: `string`
- **Description**: The situation's current state. Represented as a string equivalent of the state's enumeration value.
- **Example**: `"Unstarted"`
- **Read-only**

#### `icon`

- **Type**: `string`
- **Description**: The icon associated with the situation.
- **Example**: `"icon_path/icon_name.png"`
- **Read-only**

#### `label`

- **Type**: `string`
- **Description**: The label of the situation.
- **Example**: `"Situation Label"`
- **Read-only**

#### `description`

- **Type**: `string`
- **Description**: A brief description of the situation.
- **Example**: `"This situation describes a specific event or occurrence."`
- **Read-only**
