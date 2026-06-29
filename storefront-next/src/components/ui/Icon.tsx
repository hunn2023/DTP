import "@/lib/fontawesome";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import type { FontAwesomeIconProps } from "@fortawesome/react-fontawesome";

export default function Icon(props: FontAwesomeIconProps) {
  return <FontAwesomeIcon {...props} />;
}
