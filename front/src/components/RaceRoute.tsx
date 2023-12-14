import * as d3 from 'd3';
import { useEffect, useRef } from 'react';
import { Point, Track } from '../services/api';
import { getData } from '../services/chart';
import Button from '@mui/material/Button';
import Box from '@mui/material/Box';
import ZoomOutMapIcon from '@mui/icons-material/ZoomOutMap';

interface RaceRouteProps {
  points: Point[];
  tracks: Track[];
}

export default function RaceRoute({ points, tracks }: RaceRouteProps) {
  const refSvg = useRef<SVGSVGElement | null>(null);
  const refButton = useRef<HTMLButtonElement | null>(null);

  useEffect(() => {
    const data = getData(points, tracks);
    const zoom = d3.zoom().scaleExtent([1, Infinity]);
    const width = 1200;
    const height = 400;
    const margin = { top: 15, bottom: 3, left: 15, right: 3 };

    const svg = d3
      .select(refSvg.current as Element)
      .attr('viewBox', `0 0 ${width} ${height}`)
      .call(
        zoom.on('zoom', function (e) {
          mainGroup.transition().duration(20).attr('transform', e.transform);
        })
      )
      .on('wheel', (event) => event.preventDefault());

    const mainGroup = svg.append('g');

    d3.select('#zoomOut').on('click', function () {
      zoom.scaleBy(mainGroup as unknown as d3.Selection<Element, unknown, null, undefined>, 1);
      svg.call(zoom.transform, d3.zoomIdentity);
    });

    const xScale = d3
      .scaleLinear()
      .range([margin.left, width - margin.right])
      .domain([data.bound.x.min, data.bound.x.max]);
    const yScale = d3
      .scaleLinear()
      .range([height - margin.bottom, margin.top])
      .domain([data.bound.y.min, data.bound.y.max]);

    function updateData() {
      mainGroup.selectAll('rect').remove();
      const rects = mainGroup.selectAll('rect').data(data.rectangles);
      rects
        .enter()
        .append('rect')
        .attr('x', (r) => xScale(r.x))
        .attr('y', (r) => yScale(r.y))
        .attr('width', (r) => xScale(r.width))
        .attr('height', (r) => yScale(r.height))
        .attr('data-tooltip', (r) => r.text)
        .style('fill', (r) => r.color);
      rects.exit().remove();

      mainGroup.selectAll('line').remove();
      const lines = mainGroup.selectAll('line').data(data.lines);
      lines
        .enter()
        .append('line')
        .attr('x1', (l) => xScale(l.x1))
        .attr('y1', (l) => yScale(l.y1))
        .attr('x2', (l) => xScale(l.x2))
        .attr('y2', (l) => yScale(l.y2))
        .attr('data-tooltip', (l) => l.text)
        .style('cursor', 'pointer')
        .style('stroke', (l) => l.color)
        .style('stroke-width', 1.5);
      lines.exit().remove();

      const dotR = 4;
      mainGroup.selectAll('circle').remove();
      const dots = mainGroup.selectAll('circle').data(data.dots);
      dots
        .enter()
        .append('circle')
        .attr('cx', (d) => xScale(d.x))
        .attr('cy', (d) => yScale(d.y))
        .attr('margin-left', dotR / 2)
        .attr('margin-top', dotR / 2)
        .attr('r', dotR / 2)
        .attr('data-tooltip', (d) => d.text)
        .style('cursor', 'pointer')
        .style('fill', '#dee5e4')
        .style('stroke', '#2b2929')
        .style('stroke-width', 0.5);
      dots.exit().remove();
    }

    updateData();

    const yAxis = d3.axisLeft(yScale).ticks(10).tickSize(1);
    mainGroup.append('g').attr('transform', `translate(${margin.left},0)`).call(yAxis);

    const xAxis = d3.axisTop(xScale).ticks(20).tickSize(1);
    mainGroup.append('g').attr('transform', `translate(0, ${margin.top})`).call(xAxis);

    mainGroup
    .call((g) => g.selectAll('.domain').attr('stroke-width', 0.5))
    .call((g) => g.selectAll('.tick text').attr('font-size', 7));

    const tip = applyTooltip(mainGroup, ['circle', 'line', 'rect']);

    return () => {
      svg.selectAll('g').remove();
      tip.remove();
    };
  }, [points, tracks]);

  return (
    <Box sx={{ position: 'relative', width: '100%', border: '1px solid gray', p: '0.5rem' }}>
      <svg ref={refSvg} />
      <Button
        id="zoomOut"
        sx={{ position: 'absolute', right: '1rem', top: '1rem' }}
        ref={refButton}
        variant="contained"
        color="info"
      >
        <ZoomOutMapIcon />
      </Button>
    </Box>
  );
}

function applyTooltip(svg: d3.Selection<SVGGElement, unknown, null, undefined>, selectors: string[]) {
  const tip = d3
    .select('body')
    .append('div')
    .attr('id', 'd3Tooltip')
    .style('pointer-events', 'none')
    .style('background', '#5d5b5b')
    .style('color', '#fff')
    .style('border-radius', '3px')
    .style('padding', '3px 12px')
    .style('position', 'absolute')
    .attr('class', 'tooltip')
    .style('opacity', 0);

  for (const s of selectors) {
    svg
      .selectAll(s)
      .on('mouseover', function (d) {
        tip
          .style('opacity', 1)
          .html(d.target.getAttribute('data-tooltip'))
          .style('left', d.pageX - 30 + 'px')
          .style('top', d.pageY - 30 + 'px');
      })
      .on('mousemove', function (d) {
        tip.style('left', d.pageX - 30 + 'px').style('top', d.pageY - 30 + 'px');
      })
      .on('mouseout', function () {
        tip.style('opacity', 0);
      });
  }

  return tip;
}
