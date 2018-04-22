package com.ioanb7.trackingapp;

import android.content.Context;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;

import okhttp3.Call;
import okhttp3.FormBody;
import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;

public class MainActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        Button button = (Button)findViewById(R.id.my_button);
        button.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {

                EditText editText = (EditText) findViewById(R.id.editText);
                String id =  editText.getText().toString();

                //String url = "http://192.168.99.100/graph";
                String url = "http://192.168.99.100/api/Tracking/" + id;

                String jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYXNhc2RkMiIsIk5hbWUiOiJhc2Rhc2Rhc2QiLCJJZCI6IjIiLCJleHAiOjE1MjY3NTkwNjgsImlzcyI6InlvdXJkb21haW4uY29tIiwiYXVkIjoieW91cmRvbWFpbi5jb20ifQ.Sqd0Xi2DsWGiEVaT72YB-KO5IJogq3j7ELYekV86YYY";

                /*
                String jsonString = "{" +
                        "  books{" +
                        "    isbn," +
                        "    name" +
                        "  }" +
                        "}";*/

                String jsonString = "{\"CurrentLocation\":\"Leeds\", \"Progress\": 70}";

                RequestBody body = RequestBody.create(MediaType.parse("application/json"), jsonString);

                Request request = new Request.Builder()
                        .header("X-Client-Type", "Android")
                        .header("Authorization", "Bearer " + jwt)
                        .url(url)
                        .put(body)
                        .build();


                final Context context = getApplicationContext();

                OkHttpClient client = new OkHttpClient();
                client.newCall(request).enqueue(new okhttp3.Callback() {
                    @Override
                    public void onFailure(Call call, IOException e) {
                        e.printStackTrace();
                    }

                    @Override
                    public void onResponse(Call call, Response response) throws IOException {

                        if (!response.isSuccessful()) throw new IOException(
                                "Unexpected code " + response);

                        String r = response.body().string();
                        try {
                            JSONObject jObject = new JSONObject(r);

                            Boolean success = jObject.getBoolean("status");


                            if(success) {
                                runOnUiThread(new Runnable() {
                                    @Override
                                    public void run() {
                                        Toast.makeText(context, "Update Success", Toast.LENGTH_SHORT).show();
                                    }
                                });
                            }
                            else {
                                runOnUiThread(new Runnable() {
                                    @Override
                                    public void run() {
                                        Toast.makeText(context, "Update Success", Toast.LENGTH_SHORT).show();
                                    }
                                });
                            }
                        }catch(JSONException e) {
                            runOnUiThread(new Runnable() {
                                @Override
                                public void run() {
                                    Toast.makeText(context, "JSON parse error", Toast.LENGTH_SHORT).show();
                                }
                            });
                        }
                    }
                });










            }
        });
    }
}
