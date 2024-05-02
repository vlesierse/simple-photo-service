import { FC } from "react";
import { createPortal } from "react-dom";

import "./PortalLayout.scss";

import { PortalNavigation } from "./PortalNavigation";
import { WithChildren } from "../types";
import { DefaultLayout } from "./DefaultLayout";

const LayoutPortal: FC<WithChildren> = ({ children }) => {
  const element = document.querySelector("#h");
  return element && createPortal(children, element);
};

const PortalLayout = () => {
  return (
    <>
      <LayoutPortal>
        <PortalNavigation />
      </LayoutPortal>
      <DefaultLayout hideHeader={true} />
    </>
  );
};

export { PortalLayout };
