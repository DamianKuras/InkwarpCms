import { Button } from "@mui/material";
import type { MetaFunction } from "@remix-run/node";

export const meta: MetaFunction = () => {
  return [{ title: "Inkwarp cms" }];
};

export default function Index() {
  return (
    <div>
      <Button variant="contained">Login</Button>
    </div>
  );
}
