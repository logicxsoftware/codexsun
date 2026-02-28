import { AnimatePresence, motion } from "framer-motion"
import { LaptopMinimal, Moon, Sun } from "lucide-react"
import { useCallback, useEffect, useMemo, useRef, useState } from "react"

import { cn } from "@/lib/utils"
import { useTenantTheme } from "@/shared/theme/TenantThemeProvider"
import type { ThemePreference } from "@/shared/theme/tokens"

type ThemeOption = {
  mode: ThemePreference
  label: string
  icon: typeof Sun
}

const modeOptions: ThemeOption[] = [
  { mode: "light", label: "Light", icon: Sun },
  { mode: "dark", label: "Dark", icon: Moon },
  { mode: "system", label: "System", icon: LaptopMinimal },
]

const dropdownTransition = { duration: 0.18, ease: "easeOut" } as const

export function ThemeSwitch() {
  const { modePreference, setModePreference } = useTenantTheme()
  const [isOpen, setIsOpen] = useState(false)
  const rootRef = useRef<HTMLDivElement | null>(null)
  const itemRefs = useRef<Array<HTMLButtonElement | null>>([])

  const selectedIndex = useMemo(() => modeOptions.findIndex((option) => option.mode === modePreference), [modePreference])
  const activeOption = useMemo(() => modeOptions[selectedIndex] ?? modeOptions[2], [selectedIndex])

  const closeMenu = useCallback(() => {
    setIsOpen(false)
  }, [])

  const focusItem = useCallback((index: number) => {
    const total = modeOptions.length
    const safeIndex = ((index % total) + total) % total
    itemRefs.current[safeIndex]?.focus()
  }, [])

  const handleSelectMode = useCallback(
    (mode: ThemePreference) => {
      setModePreference(mode)
      closeMenu()
    },
    [closeMenu, setModePreference],
  )

  const handleToggle = useCallback(() => {
    setIsOpen((previous) => !previous)
  }, [])

  const handleTriggerKeyDown = useCallback(
    (event: React.KeyboardEvent<HTMLButtonElement>) => {
      if (event.key === "ArrowDown" || event.key === "ArrowUp") {
        event.preventDefault()
        setIsOpen(true)
        const targetIndex = event.key === "ArrowDown" ? selectedIndex : selectedIndex - 1
        requestAnimationFrame(() => focusItem(targetIndex))
        return
      }

      if (event.key === "Escape") {
        event.preventDefault()
        closeMenu()
      }
    },
    [closeMenu, focusItem, selectedIndex],
  )

  const handleMenuKeyDown = useCallback(
    (event: React.KeyboardEvent<HTMLDivElement>) => {
      const activeElement = document.activeElement
      const focusedIndex = itemRefs.current.findIndex((node) => node === activeElement)

      if (event.key === "Escape") {
        event.preventDefault()
        closeMenu()
        return
      }

      if (event.key === "ArrowDown") {
        event.preventDefault()
        focusItem((focusedIndex >= 0 ? focusedIndex : selectedIndex) + 1)
        return
      }

      if (event.key === "ArrowUp") {
        event.preventDefault()
        focusItem((focusedIndex >= 0 ? focusedIndex : selectedIndex) - 1)
        return
      }

      if (event.key === "Home") {
        event.preventDefault()
        focusItem(0)
        return
      }

      if (event.key === "End") {
        event.preventDefault()
        focusItem(modeOptions.length - 1)
      }
    },
    [closeMenu, focusItem, selectedIndex],
  )

  useEffect(() => {
    if (!isOpen) {
      return
    }

    const selectedNode = itemRefs.current[selectedIndex] ?? itemRefs.current[0]
    selectedNode?.focus()
  }, [isOpen, selectedIndex])

  useEffect(() => {
    if (!isOpen) {
      return
    }

    const handlePointerDown = (event: MouseEvent | TouchEvent) => {
      const target = event.target
      if (!(target instanceof Node)) {
        return
      }

      if (!rootRef.current?.contains(target)) {
        closeMenu()
      }
    }

    document.addEventListener("mousedown", handlePointerDown)
    document.addEventListener("touchstart", handlePointerDown)

    return () => {
      document.removeEventListener("mousedown", handlePointerDown)
      document.removeEventListener("touchstart", handlePointerDown)
    }
  }, [closeMenu, isOpen])

  useEffect(() => {
    if (!isOpen) {
      return
    }

    const handleWindowKeyDown = (event: KeyboardEvent) => {
      if (event.key === "Escape") {
        closeMenu()
      }
    }

    window.addEventListener("keydown", handleWindowKeyDown)
    return () => {
      window.removeEventListener("keydown", handleWindowKeyDown)
    }
  }, [closeMenu, isOpen])

  const ActiveIcon = activeOption.icon

  return (
    <div ref={rootRef} className="relative inline-flex">
      <motion.button
        type="button"
        className="inline-flex h-9 w-9 items-center justify-center rounded-full border border-border/70 bg-card/70 text-foreground transition-colors hover:bg-menu-hover focus-visible:border-ring focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring/60"
        onClick={handleToggle}
        onKeyDown={handleTriggerKeyDown}
        aria-label={`Theme mode: ${activeOption.label}`}
        aria-haspopup="menu"
        aria-expanded={isOpen}
        animate={{
          scale: isOpen ? 1.03 : 1,
        }}
        transition={{ duration: 0.18, ease: "easeOut", type: "tween" }}
        whileHover={{ scale: isOpen ? 1.03 : 1.05 }}
      >
        <AnimatePresence mode="wait" initial={false}>
          <motion.span
            key={modePreference}
            initial={{ opacity: 0, scale: 0.88 }}
            animate={{ opacity: 1, scale: 1 }}
            exit={{ opacity: 0, scale: 0.88 }}
            transition={{ duration: 0.16, ease: "easeOut", type: "tween" }}
            className="inline-flex"
          >
            <ActiveIcon className="h-4 w-4" />
          </motion.span>
        </AnimatePresence>
      </motion.button>

      <AnimatePresence>
        {isOpen ? (
          <div className="absolute left-1/2 top-11 z-50 -translate-x-1/2">
            <motion.div
              role="menu"
              aria-label="Select theme mode"
              onKeyDown={handleMenuKeyDown}
              className="min-w-12 origin-top transform-gpu rounded-xl border border-border/70 bg-popover/95 p-1 shadow-md backdrop-blur-sm will-change-transform"
              initial={{ opacity: 0, scale: 0.95 }}
              animate={{ opacity: 1, scale: 1 }}
              exit={{ opacity: 0, scale: 0.95 }}
              transition={{ ...dropdownTransition, type: "tween" }}
            >
              <div className="flex flex-col items-center gap-1">
                {modeOptions.map((option, index) => {
                  const Icon = option.icon
                  const isActive = option.mode === modePreference

                  return (
                    <motion.button
                      key={option.mode}
                      ref={(node) => {
                        itemRefs.current[index] = node
                      }}
                      type="button"
                      role="menuitem"
                      aria-label={`Switch theme to ${option.label}`}
                      className={cn(
                        "inline-flex h-9 w-9 items-center justify-center rounded-lg text-muted-foreground transition-colors focus-visible:border-ring focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring/60",
                        isActive ? "bg-menu-hover text-foreground" : "hover:bg-menu-hover hover:text-foreground",
                      )}
                      onClick={() => handleSelectMode(option.mode)}
                      whileHover={{ scale: 1.08 }}
                      whileTap={{ scale: 0.96 }}
                      transition={{ duration: 0.16, ease: "easeOut" }}
                    >
                      <Icon className="h-4 w-4" />
                    </motion.button>
                  )
                })}
              </div>
            </motion.div>
          </div>
        ) : null}
      </AnimatePresence>
    </div>
  )
}
