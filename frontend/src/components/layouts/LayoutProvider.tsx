import { FC, createContext, useContext, useState } from "react";
import { SideNavigationProps } from "@cloudscape-design/components";
import { WithChildren } from "../types";

export type NavigationItem = SideNavigationProps.Item;

export interface LayoutContextModel {
  title: string;
  setTitle: (title: string) => void;
  description: string;
  setDescription: (description: string) => void;
  navigationItems: NavigationItem[];
  setNavigationItems: (items: NavigationItem[]) => void;
}

const LayoutContext = createContext<LayoutContextModel>({
  title: "",
  setTitle: () => {},
  description: "",
  setDescription: () => {},
  navigationItems: [],
  setNavigationItems: () => {},
});

const useLayout = () => {
  return useContext(LayoutContext);
};

const LayoutProvider: FC<WithChildren> = ({ children }) => {
  const [navigationItems, setNavigationItems] = useState([] as NavigationItem[]);
  const value = {
    navigationItems,
    setNavigationItems,
  } as LayoutContextModel;
  return <LayoutContext.Provider value={value}>{children}</LayoutContext.Provider>;
};

export { LayoutContext, LayoutProvider, useLayout };
