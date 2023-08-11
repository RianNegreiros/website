import axios from 'axios';
import { CommentData, PostData, ProjectData, SignInData, SignUpData } from '../models';

const API_URL = "http://localhost:5000/api"

async function signUpUser(formData: SignUpData) {
  try {
    const response = await axios.post(`${API_URL}/user/register`, formData, {
      headers: {
        'Content-Type': 'application/json',
      },
    });

    return response.data;
  } catch (error) {
    throw new Error('Sign up failed. Please try again later.');
  }
}

async function signInUser(formData: SignInData) {
  try {
    const response = await axios.post(`${API_URL}/user/login`, formData, {
      headers: {
        'Content-Type': 'application/json',
      },
    });

    return response.data;
  } catch (error) {
    throw new Error('Sign in failed. Please try again later.');
  }
}

async function getIsAdmin() {
  try {
    const response = await axios.get(`${API_URL}/user/isadmin`, {
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${localStorage.getItem('token')}`,
      },
    });
    return response.data;
  } catch (error) {
    throw new Error('Failed to fetch user. Please try again later.');
  }
}

async function createPost(formData: PostData) {
  try {
    const response = await axios.post(`${API_URL}/post`, formData, {
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${localStorage.getItem('token')}`,
      },
    });

    return response.data;
  } catch (error) {
    throw new Error('Failed to create post. Please try again later.');
  }
}

async function autoLoginUser(token: string) {
  const response = await axios.post(
    `${API_URL}/user/autologin`,
    JSON.stringify(token),
    {
      headers: {
        'Content-Type': 'application/json',
      },
    }
  );

  return response.data;
}

async function getPosts() {
  try {
    const response = await axios.get(`${API_URL}/post`);
    return response.data;
  } catch (error) {
    throw new Error('Failed to fetch posts. Please try again later.');
  }
}

async function getPost(postId: string) {
  try {
    const response = await axios.get(`${API_URL}/post/${postId}`);
    return response.data;
  } catch (error) {
    throw new Error('Failed to fetch post. Please try again later.');
  }
}

async function getCommentsForPost(postId: string) {
  try {
    const response = await axios.get(`${API_URL}/comments/${postId}`);
    return response.data;
  } catch (error) {
    throw new Error('Failed to fetch comments. Please try again later.');
  }
}

async function addCommentToPost(commentData: CommentData) {
  try {
    const token = localStorage.getItem('token');
    commentData.token = token as string;

    if (!token) {
      throw new Error('User token not found.');
    }

    const response = await axios.post(`${API_URL}/comments/${commentData.postId}`, commentData, {
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      },
    });

    return response.data;
  } catch (error) {
    throw new Error('Failed to add comment. Please try again later.');
  }
}

async function getIsUserLoggedIn() {
  try {
    const response = await axios.get(`${API_URL}/user/me`, {
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${localStorage.getItem('token')}`,
      },
    });
    return response.data;
  } catch (error) {
    throw new Error('Failed to fetch user. Please try again later.');
  }
}

async function createProject(projectData: ProjectData) {
  try {
    const response = await axios.post(`${API_URL}/projects`, projectData, {
      headers: {
        'Content-Type': 'multipart/form-data',
        Authorization: `Bearer ${localStorage.getItem('token')}`,
      },
    });

    return response.data;
  } catch (error) {
    throw new Error('Failed to create project. Please try again later.');
  }
}

export {
  signUpUser,
  signInUser,
  getIsAdmin,
  createPost,
  autoLoginUser,
  getCommentsForPost,
  addCommentToPost,
  getPosts,
  getPost,
  getIsUserLoggedIn,
  createProject
};
