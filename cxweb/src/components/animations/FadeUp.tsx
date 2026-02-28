import { motion } from 'framer-motion';

type Props = {
    children: React.ReactNode;
    delay?: number;
};

export default function FadeUp({ children, delay = 0 }: Props) {
    return (
        <motion.div
            initial={{ opacity: 0, y: 40 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true, margin: '-80px' }}
            transition={{
                duration: 0.6,
                ease: 'easeOut',
                delay,
            }}
        >
            {children}
        </motion.div>
    );
}
