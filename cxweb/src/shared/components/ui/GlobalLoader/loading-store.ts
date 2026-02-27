type Listener = () => void

let activeCount = 0
const listeners = new Set<Listener>()

const notify = (): void => {
  for (const listener of listeners) {
    listener()
  }
}

export const subscribeGlobalLoading = (listener: Listener): (() => void) => {
  listeners.add(listener)
  return () => {
    listeners.delete(listener)
  }
}

export const getGlobalLoadingSnapshot = (): boolean => activeCount > 0

export const startGlobalLoading = (): void => {
  activeCount += 1
  notify()
}

export const stopGlobalLoading = (): void => {
  activeCount = Math.max(0, activeCount - 1)
  notify()
}

export const setGlobalLoading = (isLoading: boolean): void => {
  activeCount = isLoading ? 1 : 0
  notify()
}
