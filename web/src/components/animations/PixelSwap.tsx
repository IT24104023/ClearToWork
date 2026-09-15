import React, { useState, useRef, useEffect, type ReactNode } from 'react';


export interface PixelSwapProps {
  firstContent: ReactNode;
  secondContent: ReactNode;
  pixelSize?: number;
  gap?: number;
  pixelRadius?: number;
  pixelSpin?: number;
  pixelScale?: number;
  duration?: number;
  pixelDuration?: number;
  pattern?: 'random' | 'diagonal' | 'center' | 'spiral';
  randomness?: number;
  fade?: boolean;
  trigger?: 'hover' | 'click';
  className?: string;
}

export const PixelSwap: React.FC<PixelSwapProps> = ({
  firstContent,
  secondContent,
  pixelSize = 48,
  gap = 2,
  pixelRadius = 4,
  pixelSpin = 0,
  pixelScale = 0.4,
  duration = 1200,
  pixelDuration = 400,
  pattern = 'random',
  fade = true,
  trigger = 'hover',
  className = '',
}) => {
  const [isFlipped, setIsFlipped] = useState(false);
  const [isAnimating, setIsAnimating] = useState(false);
  const containerRef = useRef<HTMLDivElement>(null);
  const [gridDimensions, setGridDimensions] = useState({ cols: 8, rows: 6 });

  useEffect(() => {
    if (!containerRef.current) return;
    const updateGrid = () => {
      if (!containerRef.current) return;
      const { width, height } = containerRef.current.getBoundingClientRect();
      const cols = Math.max(3, Math.ceil(width / pixelSize));
      const rows = Math.max(3, Math.ceil(height / pixelSize));
      setGridDimensions({ cols, rows });
    };

    updateGrid();
    window.addEventListener('resize', updateGrid);
    return () => window.removeEventListener('resize', updateGrid);
  }, [pixelSize]);

  const handleTrigger = (type: 'hover' | 'click', state?: boolean) => {
    if (trigger !== type && trigger !== 'hover' && trigger !== 'click') return;
    if (type === 'hover') {
      setIsFlipped(Boolean(state));
      setIsAnimating(true);
    } else if (type === 'click') {
      setIsFlipped((prev) => !prev);
      setIsAnimating(true);
    }
  };

  const totalPixels = gridDimensions.cols * gridDimensions.rows;
  const pixels = Array.from({ length: totalPixels }).map((_, index) => {
    const col = index % gridDimensions.cols;
    const row = Math.floor(index / gridDimensions.cols);
    
    // Calculate delay based on pattern
    let delay = 0;
    if (pattern === 'diagonal') {
      delay = (col + row) / (gridDimensions.cols + gridDimensions.rows) * (duration - pixelDuration);
    } else if (pattern === 'center') {
      const midX = gridDimensions.cols / 2;
      const midY = gridDimensions.rows / 2;
      const dist = Math.sqrt((col - midX) ** 2 + (row - midY) ** 2);
      const maxDist = Math.sqrt(midX ** 2 + midY ** 2);
      delay = (dist / maxDist) * (duration - pixelDuration);
    } else {
      // random
      delay = Math.random() * (duration - pixelDuration);
    }

    return { id: index, col, row, delay };
  });

  return (
    <div
      ref={containerRef}
      className={`relative overflow-hidden cursor-pointer select-none rounded-2xl group transition-all duration-500 ${className}`}
      onMouseEnter={() => trigger === 'hover' && handleTrigger('hover', true)}
      onMouseLeave={() => trigger === 'hover' && handleTrigger('hover', false)}
      onClick={() => trigger === 'click' && handleTrigger('click')}
      style={{
        perspective: '1200px',
      }}
    >
      {/* Front Face */}
      <div
        className={`w-full h-full transition-all duration-700 ease-out transform ${
          isFlipped ? 'opacity-0 scale-95 pointer-events-none' : 'opacity-100 scale-100'
        }`}
      >
        {firstContent}
      </div>

      {/* Back Face */}
      <div
        className={`absolute inset-0 w-full h-full transition-all duration-700 ease-out transform ${
          isFlipped ? 'opacity-100 scale-100' : 'opacity-0 scale-95 pointer-events-none'
        }`}
      >
        {secondContent}
      </div>

      {/* Interactive Pixel Overlay on Flip */}
      {isAnimating && (
        <div
          className="absolute inset-0 pointer-events-none grid z-20"
          style={{
            gridTemplateColumns: `repeat(${gridDimensions.cols}, 1fr)`,
            gridTemplateRows: `repeat(${gridDimensions.rows}, 1fr)`,
            gap: `${gap}px`,
          }}
          onTransitionEnd={() => setIsAnimating(false)}
        >
          {pixels.map((p) => (
            <div
              key={p.id}
              className="bg-amber-500/20 backdrop-blur-[1px] border border-amber-400/30 transition-all"
              style={{
                borderRadius: `${pixelRadius}px`,
                transitionDuration: `${pixelDuration}ms`,
                transitionDelay: `${p.delay}ms`,
                opacity: isFlipped ? 0 : (fade ? 0.8 : 1),
                transform: isFlipped
                  ? `scale(${pixelScale}) rotate(${pixelSpin}deg)`
                  : 'scale(1) rotate(0deg)',
              }}
            />
          ))}
        </div>
      )}

      {/* Subtle Glow Corner */}
      <div className="absolute -top-12 -right-12 w-32 h-32 bg-amber-500/10 rounded-full blur-2xl pointer-events-none transition-opacity group-hover:opacity-100 opacity-30" />
    </div>
  );
};

export default PixelSwap;
