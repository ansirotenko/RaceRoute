## Description

Imagine that you received an order from a large technology company developing a route planning service for racing. The essence of the service is to build interesting routes over rough terrain and road infrastructure and display their characteristics. It is worth noting that the service is planned to be used both on tablets and on large TV displays.

## Task

Your task is that in this application it is necessary to display a graph with the characteristics of the intersected terrain in the route - the heights of the control points, indicate in color the maximum permissible speed on a fragment of the route, and indicate in background color the type of the intersected surface.
The X-axis is the distance traveled, the Y-axis is the height of the control point.
The component will accept a route - an array of control points.
![Pasted image 20231110094626](https://user-images.githubusercontent.com/39823399/282006397-47f018ec-6722-434f-ae31-ff2011d1b412.png)

There is no need to actually load anything from any servers, all data can simply be locked and simulated data loading by displaying a loader. It will be a plus if you implement the server side of the application - the API and any database. If desired, you can add caching, swagger, and implement a pipeline for deploying the application in docker.

In addition, the list of points and the roads connecting them must be generated independently. The data type of the point and interval can be taken below). Changing these types of data is ** prohibited**.

As a completed work, you need to upload the project to GitHub Pages and provide a link to view both the project and the source code.

## Technology requirements
- React **without using class components**
- React Hooks
- It is advisable to use the library mui.com
- The minimum screen width is 320px. Maximum 7680px
- It is advisable to use JSS rather than CSS. To do this, you can use the [@material-ui/styles] library (https://www.npmjs.com/package/@material-ui/styles )_
- On the back of the EF Core 6 or 7 version

## Visual requirements

- The entire route must be displayed on the graph. If there are more than 20 points, you can provide for scaling the graph.
- The application should not slow down even when there are more than 100 dots on the screen
- When hovering over a point, a popup with information about the point should appear
- The completed work must be visually close to the above layout.

## Data types
```js
// List of possible surface types
enum Surface {
SAND,
ASPHALT,
GROUND
}
enum MaxSpeed {
FAST,
NORMAL,
SLOW
}

// Control point. The position in the route is defined in the route array
interface Point {
// id
id: number;
// name
name: string;
// Point height
height: number;
}

// Segment. Defines the characteristics of the route section between 2 adjacent
interface Track points {
// id of the first point
firstId: number;
// id of the second point
secondId: number;
// distance between points
distance: number;
// type of surface on the segment
surface: Surface;
// maximum permissible speed on the segment
maxSpeed: MaxSpeed;
}
```