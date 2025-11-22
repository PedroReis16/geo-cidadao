import type { Post } from "../../../data/@types/Post";

interface PostDetailsCardProps {
  post: Post;
}

const PostDetailsCard: React.FC<PostDetailsCardProps> = ({ post }) => {
  return <div className="content-card"></div>;
};
export default PostDetailsCard;
