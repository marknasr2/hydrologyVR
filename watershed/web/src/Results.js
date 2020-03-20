import React  from 'react';
import {
    DataTable,
    Card,
    Layout,
} from '@shopify/polaris';

const Results = (rows, handleBack) => (
    <Layout>
        <Layout.Section>
            <Card sectioned title="Predictions" actions={[{ content: 'Back', onAction: handleBack }]}>
                <DataTable
                    columnContentTypes={[
                        'text',
                        'numeric',
                        'numeric',
                    ]}
                    headings={[
                        'Output Name',
                        'Previous Value',
                        'Value',
                    ]}
                    rows={rows}
                />
            </Card>
        </Layout.Section>
    </Layout>
);

export default Results;