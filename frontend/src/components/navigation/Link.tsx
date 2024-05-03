import { Link, LinkProps } from "@cloudscape-design/components";
import { useNavigate } from "react-router-dom";

const NavigateLink = (props: LinkProps) => {
  const navigate = useNavigate();
  const onFollowHandler = (event: CustomEvent<LinkProps.FollowDetail>) => {
    if (props.href && !event.detail.external && props.target !== "_blank") {
      event.preventDefault();
      navigate(props.href);
    }
  };
  return <Link {...props} onFollow={onFollowHandler} />;
};

export { NavigateLink as Link };
