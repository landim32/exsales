import { useContext, useEffect, useState } from "react";
import Col from "react-bootstrap/esm/Col";
import Container from "react-bootstrap/esm/Container";
import Row from "react-bootstrap/esm/Row";
import { Link, useNavigate } from "react-router-dom";
import ReactQuill from "react-quill";
import {
    Editor,
    Frame,
    Element,
    useEditor,
    useNode,
} from "@craftjs/core";
import "react-quill/dist/quill.snow.css";
import { CustomToolbar } from "../../Components/CustomToolbar";
import Products from "./Products";

export default function NetworkPage() {

    let navigate = useNavigate();



    // Componente editável com Bootstrap
    const HeaderText = () => {
        const {
            connectors: { connect, drag },
            actions: { setProp },
            props,
        } = useNode((node) => ({
            props: node.data.props,
        }));

        const { query } = useEditor();

        const [editorContent, setEditorContent] = useState(props.html || "");

        useEffect(() => {
            setProp((props: any) => (props.html = editorContent));
        }, [editorContent, setProp]);

        const handleSave = () => {
            const json = query.serialize();
        };

        return (
            <div ref={(ref) => connect(drag(ref))} className="p-3 bg-light">
                <CustomToolbar onSave={handleSave} />
                <ReactQuill
                    theme="snow"
                    value={editorContent}
                    onChange={setEditorContent}
                    modules={{
                        toolbar: {
                            container: "#custom-toolbar",
                        },
                    }}
                    formats={[
                        "header",
                        "bold",
                        "italic",
                        "underline",
                        "size",
                        "link",
                        "clean",
                    ]}
                />
            </div>
        );

    };

    HeaderText.craft = {
        props: { html: "<h2>Minha Rede Principal</h2><p>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum</p>" },
        displayName: "HeaderText",
    };

    return (
        <>
            <Container>
                <Row>
                    <Col md="12">
                        <Editor resolver={{ HeaderText }}>
                            <Frame>
                                <Element is="div" canvas>
                                    <HeaderText />
                                </Element>
                            </Frame>
                        </Editor>
                    </Col>
                </Row>
                <Row>
                    <Col md="12">
                        <Products />
                    </Col>
                </Row>
            </Container>
        </>
    );
}