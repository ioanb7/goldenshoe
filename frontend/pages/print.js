
import Layout from '../components/MyLayout.js'
import Link from 'next/link'

export default () => (
    <Layout>
        <h1>Print</h1>
        <div className="content">
            <p>Do you know what size fits you? Download the below PDF to make sure before purchase.</p>
            
            <Link href="/print-download" >
                <a className="button">Download here</a>
            </Link>
        </div>
    </Layout>
)