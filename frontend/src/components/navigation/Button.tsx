import { Button, ButtonProps } from "@cloudscape-design/components";
import { useNavigate } from "react-router-dom";

const NavigateButton = (props: ButtonProps) => {
  const navigate = useNavigate();
  const onFollowHandler = (event: CustomEvent<ButtonProps.FollowDetail>) => {
    if (props.href && props.target !== "_blank") {
      event.preventDefault();
      navigate(props.href);
    }
  };
  return <Button {...props} onFollow={onFollowHandler} />;
};

export { NavigateButton as Button };
