import { Outlet } from "react-router-dom";
import { AppLayout, Box, ContentLayout, Header, SpaceBetween } from "@cloudscape-design/components";
import { ReactComponent as Logo } from "../../assets/images/aws-logo.svg";

export type DefaultLayoutProps = {
  hideHeader?: boolean;
};
export const DefaultLayout = ({ hideHeader }: DefaultLayoutProps) => {
  return (
    <AppLayout
      content={
        <ContentLayout
          header={
            !hideHeader && (
              <SpaceBetween direction="horizontal" size="xl">
                <Box variant="span" padding={{ left: "l", right: "l", top: "xxl" }}>
                  <Logo width={75} />
                </Box>
                <Header variant="h1" description="Store and share your photos on AWS">
                  Simple Photo Service
                </Header>
              </SpaceBetween>
            )
          }
        >
          <Outlet />
        </ContentLayout>
      }
      toolsHide
      navigationHide
    />
  );
};
