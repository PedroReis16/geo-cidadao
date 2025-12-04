export interface Message {
  message: string;
  type: "success" | "info" | "error" | "secondary" | "contrast" | "danger" | "help" | "primary" | null | undefined;
  duration?: number;
  links?: {
    label: string;
    url: string;
  }[];
}