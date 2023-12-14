import { ChartData, Dot, Line, MaxSpeed, Point, Rectangle, Surface, Track } from './api';

export function getData(points: Point[], tracks: Track[]): ChartData {
  const y = points.map((p) => p.height);
  const x = [0];
  tracks.forEach((t, i) => {
    const delta = Math.sqrt(Math.pow(t.distance, 2) - Math.pow(y[i] - y[i + 1], 2));
    return x.push(delta + x[i]);
  });

  const yMax = Math.max(...y) * 1.15;

  const rectangles: Rectangle[] = tracks.map((t, i) => {
    const surfInfo = getSurfaceInfo(t.surface);
    return {
      x: x[i],
      width: x[i + 1] - x[i],
      y: yMax,
      height: 0,
      color: surfInfo.color,
      text: surfInfo.name,
    };
  }).reduce((acc, t) => {
    if (!acc.length || acc[acc.length - 1].color !== t.color) {
        acc.push(t);
    } else {
        acc[acc.length - 1].width += t.width;
    }

    return acc;
  }, [] as (Rectangle)[])

  const dots: Dot[] = points.map((p, i) => ({
    x: x[i],
    y: y[i],
    text: `height: ${Math.floor(p.height * 10) / 10}  ${p.name}`,
  }));

  const lines: Line[] = tracks.map((t, i) => {
    const speedInfo = getSpeedInfo(t.maxSpeed);
    return {
      x1: x[i],
      x2: x[i + 1],
      y1: y[i],
      y2: y[i + 1],
      color: speedInfo.color,
      text: `distance: ${Math.floor(t.distance * 10) / 10} ${speedInfo.name}`
    };
  });

  return {
    dots: dots,
    lines: lines,
    rectangles: rectangles,
    bound: {
      x: {
        min: x[0],
        max: x[x.length - 1],
      },
      y: {
        min: 0,
        max: yMax,
      },
    },
  };
}

function getSurfaceInfo(surface: Surface) {
  switch (surface) {
    case Surface.SAND:
      return {
        name: 'Sand',
        color: '#ffeba8',
      };
    case Surface.ASPHALT:
      return {
        name: 'Asphalt',
        color: '#9d9d9d',
      };
    case Surface.GROUND:
      return {
        name: 'Ground',
        color: '#7cdb86',
      };
  }
}

function getSpeedInfo(maxSpeed: MaxSpeed) {
  switch (maxSpeed) {
    case MaxSpeed.FAST:
      return {
        name: 'Fast',
        color: 'green',
      };
    case MaxSpeed.NORMAL:
      return {
        name: 'Normal',
        color: 'yellow',
      };
    case MaxSpeed.SLOW:
      return {
        name: 'Slow',
        color: 'red',
      };
  }
}
