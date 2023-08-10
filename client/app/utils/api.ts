import axios from 'axios';
import { PostData, SignInData, SignUpData } from '../models';

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
    const response = await axios.get(`${API_URL}/user/isadmin`, {});
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

async function autoLoginUser(persistentToken: string): Promise<SignInData> {
  try {
      const response = await axios.post<SignInData>('/api/autologin', {
          persistentToken: persistentToken
      });

      return response.data;
  } catch (error) {
      throw new Error('Auto-login failed.');
  }
}

export { signUpUser, signInUser, getIsAdmin, createPost, autoLoginUser };
