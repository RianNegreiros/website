'use client'

import Link from 'next/link'
import { usePathname } from 'next/navigation'

export default function Footer() {
  const pathname = usePathname()

  const paths = ['/signin', '/signup', '/terms/service', '/terms/privacy']

  if (paths.includes(pathname)) {
    return null
  }

  return (
    <footer className='mt-auto bg-white dark:bg-gray-900'>
      <div className='mx-auto w-full max-w-screen-xl p-4 py-6 lg:py-8'>
        <hr className='my-6 border-gray-200 dark:border-gray-700 sm:mx-auto lg:my-8' />
        <div className='sm:flex sm:items-center sm:justify-between'>
          <span className='text-sm text-gray-700 dark:text-gray-400 sm:text-center'>
            © {new Date().getFullYear()}{' '}
            <Link
              href='https://www.linkedin.com/in/riannegreiros'
              className='text-purple-500 hover:underline font-semibold'
              aria-label='Rodapé da página Abrir LinkedIn'
            >
              Rian Negreiros™
            </Link>
            . Todos os direitos reservados.
          </span>

          <ul className='mb-6 flex flex-wrap items-center text-sm font-medium text-gray-700 dark:text-gray-400 sm:mb-0'>
            <li>
              <a
                href='/terms/privacy'
                className='mr-4 text-purple-500 hover:underline font-semibold md:mr-6'
              >
                Política Privacidade
              </a>
            </li>
            <li>
              <a
                href='/terms/service'
                className='mr-4 text-purple-500 hover:underline font-semibold md:mr-6'
              >
                Termos de Serviço
              </a>
            </li>
          </ul>
        </div>
      </div>
    </footer>
  )
}