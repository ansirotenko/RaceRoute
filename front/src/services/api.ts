export enum Surface {
  SAND,
  ASPHALT,
  GROUND,
}

export enum MaxSpeed {
  FAST,
  NORMAL,
  SLOW,
}

export interface Point {
  id: number;
  name: string;
  height: number;
}

export interface Track {
  firstId: number;
  secondId: number;
  distance: number;
  surface: Surface;
  maxSpeed: MaxSpeed;
}

export interface Race {
  id: number;
  name: string;
}

export interface GenerateArgs {
  heightMean: number;
  heightStddev: number;
  distanceMean: number;
  distanceStddev: number;
  surfaceSmoothness: number;
  speedSmoothness: number;
  pointsNumber: number;
}

export type Dot = {
  x: number;
  y: number;
  text: string;
};

export type Line = {
  x1: number;
  y1: number;
  x2: number;
  y2: number;
  color: string;
  text: string;
};

export type Rectangle = {
  x: number;
  y: number;
  width: number;
  height: number;
  color: string;
  text: string;
};

export type Range = {
  min: number;
  max: number;
};

export type ChartData = {
  dots: Dot[];
  lines: Line[];
  rectangles: Rectangle[];
  bound: {
    x: Range;
    y: Range;
  };
};
