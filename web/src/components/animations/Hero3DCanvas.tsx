import React, { useRef, useEffect } from 'react';

export interface Hero3DCanvasProps {
  className?: string;
}

export const Hero3DCanvas: React.FC<Hero3DCanvasProps> = ({ className = '' }) => {
  const canvasRef = useRef<HTMLCanvasElement>(null);

  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;

    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    let animationFrameId: number;
    let width = (canvas.width = canvas.parentElement?.clientWidth || window.innerWidth);
    let height = (canvas.height = canvas.parentElement?.clientHeight || window.innerHeight);

    const handleResize = () => {
      if (!canvas || !canvas.parentElement) return;
      width = canvas.width = canvas.parentElement.clientWidth;
      height = canvas.height = canvas.parentElement.clientHeight;
    };
    window.addEventListener('resize', handleResize);

    // Mouse coordinates
    let mouseX = width / 2;
    let mouseY = height / 2;
    let targetMouseX = width / 2;
    let targetMouseY = height / 2;

    const handleMouseMove = (e: MouseEvent) => {
      const rect = canvas.getBoundingClientRect();
      targetMouseX = e.clientX - rect.left;
      targetMouseY = e.clientY - rect.top;
    };
    window.addEventListener('mousemove', handleMouseMove);

    // 3D Nodes
    const NODE_COUNT = 48;
    interface Node3D {
      x: number;
      y: number;
      z: number;
      vx: number;
      vy: number;
      vz: number;
      radius: number;
      color: string;
    }

    const nodes: Node3D[] = [];
    const colors = ['#f59e0b', '#fbbf24', '#38bdf8', '#818cf8', '#34d399'];

    for (let i = 0; i < NODE_COUNT; i++) {
      nodes.push({
        x: (Math.random() - 0.5) * width * 1.2,
        y: (Math.random() - 0.5) * height * 1.2,
        z: Math.random() * 400 + 100,
        vx: (Math.random() - 0.5) * 0.4,
        vy: (Math.random() - 0.5) * 0.4,
        vz: (Math.random() - 0.5) * 0.2,
        radius: Math.random() * 2.5 + 1.5,
        color: colors[Math.floor(Math.random() * colors.length)],
      });
    }

    const fov = 350;

    const render = () => {
      // Smooth cursor lerp
      mouseX += (targetMouseX - mouseX) * 0.05;
      mouseY += (targetMouseY - mouseY) * 0.05;

      ctx.clearRect(0, 0, width, height);

      // Camera tilt based on mouse
      const rotationY = ((mouseX - width / 2) / width) * 0.35;
      const rotationX = -((mouseY - height / 2) / height) * 0.35;

      const projectedNodes: { px: number; py: number; scale: number; node: Node3D }[] = [];

      // Update & Project 3D to 2D
      for (let i = 0; i < nodes.length; i++) {
        const n = nodes[i];

        // Move
        n.x += n.vx;
        n.y += n.vy;
        n.z += n.vz;

        // Boundary wrap
        if (n.x > width) n.x = -width;
        if (n.x < -width) n.x = width;
        if (n.y > height) n.y = -height;
        if (n.y < -height) n.y = height;
        if (n.z > 500) n.z = 100;
        if (n.z < 100) n.z = 500;

        // 3D Rotations
        // Around Y axis
        const cosY = Math.cos(rotationY);
        const sinY = Math.sin(rotationY);
        const x1 = n.x * cosY + n.z * sinY;
        const z1 = -n.x * sinY + n.z * cosY;

        // Around X axis
        const cosX = Math.cos(rotationX);
        const sinX = Math.sin(rotationX);
        const y2 = n.y * cosX - z1 * sinX;
        const z2 = n.y * sinX + z1 * cosX;

        // Perspective division
        const scale = fov / (fov + z2);
        const px = x1 * scale + width / 2;
        const py = y2 * scale + height / 2;

        if (z2 > -fov + 10) {
          projectedNodes.push({ px, py, scale, node: n });
        }
      }

      // Draw Safety Grid Lines connecting nearby nodes
      ctx.lineWidth = 0.75;
      for (let i = 0; i < projectedNodes.length; i++) {
        for (let j = i + 1; j < projectedNodes.length; j++) {
          const p1 = projectedNodes[i];
          const p2 = projectedNodes[j];
          const dx = p1.px - p2.px;
          const dy = p1.py - p2.py;
          const dist = Math.sqrt(dx * dx + dy * dy);

          if (dist < 140) {
            const alpha = (1 - dist / 140) * 0.25 * Math.min(p1.scale, p2.scale);
            ctx.strokeStyle = `rgba(245, 158, 11, ${alpha})`;
            ctx.beginPath();
            ctx.moveTo(p1.px, p1.py);
            ctx.lineTo(p2.px, p2.py);
            ctx.stroke();
          }
        }
      }

      // Draw Nodes
      for (let i = 0; i < projectedNodes.length; i++) {
        const { px, py, scale, node } = projectedNodes[i];
        const r = Math.max(1, node.radius * scale);
        const alpha = Math.min(1, scale * 1.2);

        // Node Glow
        ctx.fillStyle = node.color;
        ctx.globalAlpha = alpha * 0.8;
        ctx.beginPath();
        ctx.arc(px, py, r, 0, Math.PI * 2);
        ctx.fill();

        // Outer pulse circle on some nodes
        if (i % 5 === 0) {
          ctx.strokeStyle = node.color;
          ctx.globalAlpha = alpha * 0.3;
          ctx.beginPath();
          ctx.arc(px, py, r * 2.5, 0, Math.PI * 2);
          ctx.stroke();
        }
      }

      ctx.globalAlpha = 1.0;
      animationFrameId = requestAnimationFrame(render);
    };

    render();

    return () => {
      window.removeEventListener('resize', handleResize);
      window.removeEventListener('mousemove', handleMouseMove);
      cancelAnimationFrame(animationFrameId);
    };
  }, []);

  return (
    <canvas
      ref={canvasRef}
      className={`absolute inset-0 pointer-events-none w-full h-full ${className}`}
    />
  );
};

export default Hero3DCanvas;
