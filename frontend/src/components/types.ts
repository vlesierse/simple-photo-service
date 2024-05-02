import { ReactNode } from "react";

export type Optional<T, K extends keyof T> = Pick<Partial<T>, K> & Omit<T, K>;

export type WithChildren = { children?: ReactNode };
