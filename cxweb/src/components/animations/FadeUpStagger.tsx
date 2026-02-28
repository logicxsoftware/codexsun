// resources/js/components/animate/fade-up-stagger.tsx
'use client';

import { motion } from 'framer-motion';
import type { ReactNode } from 'react';

interface FadeUpStaggerProps {
    children: ReactNode;
    staggerChildren?: number;
    delay?: number;
    className?: string;
    once?: boolean;
}

export default function FadeUpStagger({
    children,
    staggerChildren = 0.12,
    delay = 0,
    className = '',
    once = true,
}: FadeUpStaggerProps) {
    return (
        <motion.div
            initial="hidden"
            whileInView="visible"
            viewport={{ once, amount: 0.25 }}
            variants={{
                hidden: { opacity: 0 },
                visible: {
                    opacity: 1,
                    transition: {
                        staggerChildren,
                        delayChildren: delay,
                    },
                },
            }}
            className={className}
        >
            {children}
        </motion.div>
    );
}

export function FadeUpItem({
    children,
    className = '',
}: {
    children: ReactNode;
    className?: string;
}) {
    return (
        <motion.div
            variants={{
                hidden: { opacity: 0, y: 45 },
                visible: {
                    opacity: 1,
                    y: 0,
                    transition: {
                        duration: 0.75,
                        ease: [0.16, 1, 0.3, 1],
                    },
                },
            }}
            className={className}
        >
            {children}
        </motion.div>
    );
}
