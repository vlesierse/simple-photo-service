import { TopNavigation } from "@cloudscape-design/components";
import { useAuth } from "../../modules/auth/core";

export const PortalNavigation = () => {
  const { currentUser } = useAuth();
  const profileActions = [
    {
      type: "button",
      id: "signout",
      text: "Sign out",
      href: "/logout",
    },
  ];
  return (
    <TopNavigation
      identity={{ href: "/", title: "Simple Photo Service" }}
      utilities={[
        {
          type: "menu-dropdown",
          text: currentUser?.signInDetails?.loginId,
          iconName: "user-profile",
          items: profileActions,
        },
      ]}
    />
  );
};
